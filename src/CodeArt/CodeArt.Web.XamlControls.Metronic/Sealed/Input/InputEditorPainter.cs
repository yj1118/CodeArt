using CodeArt.HtmlWrapper;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Sealed;
using CodeArt.Web.XamlControls.Bootstrap;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputEditorPainter : InputTextPainter
    {
        public InputEditorPainter() { }

      
        private static string GetKey(HtmlNode node)
        {
            return node.Attributes["id"].Value;
        }

        protected override void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            UIUtil.CheckProperties(node, "id");
            FillControl(obj, node, coreNode, brush);
        }

        protected override void FillControl(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            string width = node.GetAttributeValue("width", "100%");
            string height = node.GetAttributeValue("height", "400px");
            string id = GetKey(node);

            brush.DrawLine("<script>");
            brush.DrawLine("function _editor_init_" + id + "(){");
            brush.DrawLine("tinymce.init({");
            brush.DrawLine("init_instance_callback: function(){ $$('#" + id + "').inited(true);},");
            brush.DrawLine("language: 'zh_CN',");
            brush.DrawFormat("selector: 'textarea#tinymce{0}',", id);
            brush.DrawLine();
            brush.Draw("theme: \"modern\",");
            brush.DrawFormat("width: '{0}',height: '{1}',", width, height);
            brush.DrawLine();
            WriteTool(node, brush);
            WriteFont(node, brush);
            WriteTemplates(node, brush);
            WriteContentCss(node, brush);
            brush.DrawLine("});");
            brush.DrawLine("}");
            brush.DrawLine("</script>");
            brush.DrawFormat("<textarea id=\"tinymce{0}\" name=\"tinymce{0}\"></textarea>", id);

            string toolbar = node.GetAttributeValue("toolbar", string.Empty);
            if (toolbar.IndexOf(" disk ") > -1)
            {
                var diskNode = node.SelectSingleNodeEx("core/disk");
                if (diskNode != null)
                {
                    string rootId = diskNode.GetAttributeValue("rootId", string.Empty);
                    string target = diskNode.GetAttributeValue("target", string.Empty);
                    string diskHeight = diskNode.GetAttributeValue("height", "500px");


                    StringBuilder code = new StringBuilder();
                    code.AppendFormat("<m:modal xmlns=\"http://schemas.codeart.cn/web/xaml\" xmlns:m=\"http://schemas.codeart.cn/web/xaml/metronic\" id=\"editor-disk-modal-{0}\" size=\"full\">", id);
                    code.AppendLine();
                    code.AppendFormat("<m:disk id=\"editor-disk-{0}\" name=\"disk\" target=\"{1}\" rootId=\"{2}\" height=\"{3}\"></m:disk>", id, target, rootId, diskHeight);
                    code.AppendLine();
                    code.AppendLine("<m:modal.footer>");
                    code.AppendLine("<m:button color=\"default\" data-dismiss=\"modal\">关闭</m:button>");
                    code.AppendLine("</m:modal.footer>");
                    code.AppendLine("</m:modal>");

                    obj.Elements.Render(brush, code.ToString());


                    //brush.DrawFormat("<metro:modal id=\"editor-disk-modal-{0}\" size=\"full\">", id);
                    //brush.DrawLine();
                    //brush.DrawLine("<body>");
                    //brush.DrawFormat("<metro:disk id=\"editor-disk-{0}\" target=\"{1}\" rootId=\"{2}\" height=\"{3}\"></metro:disk>", id, target, rootId, diskHeight);
                    //brush.DrawLine();
                    //brush.DrawLine("</body>");
                    //brush.DrawLine("<footer>");
                    //brush.DrawLine("<metro:button color=\"default\" data-dismiss=\"modal\">取消关闭</metro:button>");
                    //brush.DrawLine("</footer>");
                    //brush.DrawLine("</metro:modal>");
                }
            }
        }

        private static void WriteContentCss(HtmlNode node, PageBrush brush)
        {
            string contentCss = node.GetAttributeValue("contentCss", string.Empty);
            StringBuilder path = new StringBuilder("/assets/tinymce/css/content.css");
            if (!string.IsNullOrEmpty(contentCss)) path.AppendFormat(",{0}", contentCss);
            brush.DrawFormat("content_css: \"{0}\"", path.ToString());
            brush.DrawLine();
        }

        private static void WriteTool(HtmlNode node, PageBrush brush)
        {
            string toolbar = node.GetAttributeValue("toolbar", string.Empty);
            string menubar = node.GetAttributeValue("menubar", _defaultMenubar);
            string plugins = node.GetAttributeValue("plugins", string.Empty);

            if (plugins.Length == 0) plugins = _defaultPlugins;
            if (toolbar.Length == 0) toolbar = _defaultToolbar;

            brush.DrawFormat("plugins: [{0}],", plugins);
            brush.DrawLine();
            if (toolbar == "false")
                brush.Draw("toolbar: false,");
            else
                brush.DrawFormat("toolbar: \"{0}\",", toolbar);
            if (menubar == "false")
                brush.Draw("menubar: false,");
            else
                brush.DrawFormat("menubar: \"{0}\",", menubar);
            WriteFileManager(node, brush, toolbar);
            brush.DrawLine();
        }

        private static void WriteFileManager(HtmlNode node, PageBrush brush, string toolbar)
        {
            if (toolbar.IndexOf(" disk ") > -1)
            {
                var diskNode = node.SelectSingleNodeEx("core/disk");
                if (diskNode == null) return;
                string id = node.GetAttributeValue("id", string.Empty);
                brush.DrawFormat("editor_id:\"{0}\",", id);
                brush.Draw("disk_title:\"文件空间\" ,");
            }
        }


        private const string _defaultToolbar = @"fontselect | fontsizeselect | formatselect | bold italic underline strikethrough | forecolor backcolor subscript superscript blockquote removeformat "
                                                + " | ltr rtl | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | table image media | link unlink | charmap emoticons inserttime hr pagebreak "
                                                + " | undo redo cut copy paste pastetext searchreplace | preview fullscreen visualblocks code | template insertCode disk";

        private const string _defaultPlugins = @"'advlist autolink link image lists charmap print preview hr anchor pagebreak spellchecker',"
                                                      + "'searchreplace visualblocks visualchars code fullscreen insertdatetime media nonbreaking',"
                                                      + "'save table contextmenu directionality emoticons template paste textcolor',"
                                                      + "'insertCode disk'";

        private const string _defaultMenubar = "false";

        private static void WriteFont(HtmlNode node, PageBrush brush)
        {
            string fontFamily = node.GetAttributeValue("fontFamily", _defaultFontFamily);
            string fontSize = node.GetAttributeValue("fontSize", _defaultFontSize);

            brush.DrawFormat("font_formats: \"{0}\",", fontFamily);
            brush.DrawLine();
            brush.DrawFormat("fontsize_formats: \"{0}\",", fontSize);
            brush.DrawLine();
        }

        private const string _defaultFontFamily = @"宋体=SimSun;" +
                                                    "黑体=SimHei;" +
                                                    "微软雅黑=Microsoft YaHei;" +
                                                    "微软正黑体=Microsoft JhengHei;" +
                                                    "微软雅黑UI= Microsoft YaHei UI;" +
                                                    "新宋体=NSimSun;" +
                                                    "新细明体=PMingLiU;" +
                                                    "细明体=MingLiU;" +
                                                    "标楷体=DFKai-SB;" +
                                                    "仿宋=FangSong;" +
                                                    "楷体=KaiTi;" +
                                                    "仿宋_GB2312=FangSong_GB2312;" +
                                                    "楷体_GB2312=KaiTi_GB2312;" +
                                                    "Andale Mono=andale mono,times;" +
                                                    "Arial=arial,helvetica,sans-serif;" +
                                                    "Arial Black=arial black,avant garde;" +
                                                    "Book Antiqua=book antiqua,palatino;" +
                                                    "Comic Sans MS=comic sans ms,sans-serif;" +
                                                    "Courier New=courier new,courier;" +
                                                    "Georgia=georgia,palatino;" +
                                                    "Helvetica=helvetica;" +
                                                    "Impact=impact,chicago;" +
                                                    "Symbol=symbol;" +
                                                    "Tahoma=tahoma,arial,helvetica,sans-serif;" +
                                                    "Terminal=terminal,monaco;" +
                                                    "Times New Roman=times new roman,times;" +
                                                    "Trebuchet MS=trebuchet ms,geneva;" +
                                                    "Verdana=verdana,geneva";

        private const string _defaultFontSize = "8px 10px 12px 14px 16px 18px 20px 22px 24px 28px 32px 36px";

        private static void WriteTemplates(HtmlNode node, PageBrush brush)
        {
            brush.DrawLine("templates:[");
            var templates = node.SelectNodesEx("templates/add");
            foreach (HtmlNode item in templates)
            {
                string title = item.GetAttributeValue("title", string.Empty);
                string description = item.GetAttributeValue("description", string.Empty);
                string content = item.GetAttributeValue("content", string.Empty);
                string url = item.GetAttributeValue("url", string.Empty);
                brush.Draw("{");
                if (!string.IsNullOrEmpty(title)) brush.DrawFormat("title:\"{0}\",", title);
                if (!string.IsNullOrEmpty(title)) brush.DrawFormat("description:\"{0}\",", description);
                if (!string.IsNullOrEmpty(title)) brush.DrawFormat("content:\"{0}\",", content);
                if (!string.IsNullOrEmpty(title)) brush.DrawFormat("url:\"{0}\",", url);
                brush.Draw("},");
            }

            //if (templates.Count > 0) html.Length--;

            brush.DrawLine("],");

        }

        public static new readonly InputEditorPainter Instance = new InputEditorPainter();
    }
}
