using CodeArt.HtmlWrapper;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Sealed;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputThumbnailsPainter : InputBasePainter
    {
        public InputThumbnailsPainter() { }

        public override void FillHtml(object obj, HtmlNode node, PageBrush brush)
        {
            UIUtil.CheckProperties(node, "id", "type");
            if (node.Attributes["name"] == null) node.SetAttributeValue("name", node.GetAttributeValue("id", string.Empty));

            SealedPainter.CreateNodeCode(brush, "div", UIUtil.GetClassName(node, "portlet box green thumbnails"), SealedPainter.GetStyleCode(node), GetProxyCode(obj, node), (pageBrush) =>
            {
                pageBrush.DrawLine("<div class=\"portlet-title\">");
                pageBrush.DrawFormat("<div class=\"caption\"><i class=\"fa fa-cogs\"></i>{0}</div>", GetLabel(node));
                pageBrush.DrawLine();
                pageBrush.DrawLine("<div class=\"tools\"><a href=\"javascript:;\" class=\"collapse\"></a></div>");
                pageBrush.DrawLine("</div>");
                pageBrush.DrawLine("<div class=\"portlet-body\" style=\"overflow: hidden;\">");
                pageBrush.DrawFormat("    <div class=\"note note-info\"><p>{0}</p></div>", GetHelp(node));
                pageBrush.DrawLine();
                pageBrush.DrawLine("    <div class=\"row\" data-name=\"thumbnails\" data-proxy=\"{give:new $$.databind()}\">");
                pageBrush.DrawLine("    <div class=\"col-lg-12 col-auto\" data-proxy=\"{loops:'items',onbind:$$.component.input.thumbnails.itemBind}\">");
                pageBrush.DrawLine("       <div class=\"thumbnail\">");
                pageBrush.DrawLine("           <img src=\"#\" />");
                pageBrush.DrawLine("           <div class=\"caption\">");
                pageBrush.DrawLine("               <h3 class=\"text-center\" data-proxy=\"{binds:{text:'name'}}\"></h3>");
                pageBrush.Draw("               <p class=\"text-center\">");

                if (node.SelectSingleNodeEx("core/upload") != null)
                {
                    pageBrush.Draw("<button class=\"btn red btn-sm thumbnails-btn-upload\">上传原图</button>");
                }
                if (node.SelectSingleNodeEx("core/cropper") != null)
                {
                    pageBrush.Draw("<button class=\"btn green btn-sm thumbnails-btn-cropper\">快捷上传</button>");
                }
                pageBrush.Draw("<button class=\"btn default btn-sm thumbnails-btn-remove\">移除图片</button>");
                pageBrush.DrawLine("</p>");
                pageBrush.DrawLine("           </div>");
                pageBrush.DrawLine("       </div>");
                pageBrush.DrawLine("   </div>");
                pageBrush.DrawLine("   </div>");
                pageBrush.DrawLine("</div>");
            }, () =>
            {
                return UIUtil.GetProperiesCode(node, "id", "name", "data-name");
            });

            //StringBuilder code = new StringBuilder();
            //code.AppendLine(segmentHtml);
            FillCropperHtml(obj as SealedControl, node, brush);
            FillUploadHtml(obj as SealedControl, node, brush);
            //return code.ToString();
        }

        private void FillCropperHtml(SealedControl obj, HtmlNode node, PageBrush brush)
        {
            if (node.SelectSingleNodeEx("core/cropper") == null) return;
            string modalId = string.Format("selectCoverModal_cropper_{0}", node.GetAttributeValue("id", string.Empty));

            StringBuilder modal = new StringBuilder();

            modal.AppendFormat("<m:modal id=\"{0}\" size=\"full\" title=\"快捷上传\">", modalId);
            modal.AppendLine();
            modal.AppendFormat("<m:cropper maxfilesize=\"{0}\" name=\"cropper\" target=\"{1}\" disk=\"true\" allowMode=\"{2}\" id=\"cropper_{3}\"></m:cropper>",
            //modal.AppendFormat("<ms:cropper maxfilesize=\"{0}\" target=\"{1}\" allowMode=\"{2}\" id=\"cropper_{3}\"></ms:cropper>",
                                                                                                           GetCropperPara(node, "maxfilesize", "40960"),
                                                                                                           GetCropperPara(node, "target", string.Empty),
                                                                                                           GetCropperPara(node, "allowMode", "false"),
                                                                                                           node.GetAttributeValue("id", string.Empty));
            modal.AppendLine();
            modal.AppendLine("    <m:modal.footer>");
            modal.AppendLine("        <m:button class=\"thumbnails-cropper-btn-ok\" color=\"blue\" icon=\"check\">确定上传选区图片</m:button>");
            modal.AppendLine("        <m:button color=\"default\" data-dismiss=\"modal\">取消关闭</m:button>");
            modal.AppendFormat("        <m:alert id=\"thumbnailsAlert_cropper_{0}\" style=\"margin-top:20px;\"></m:alert>", node.GetAttributeValue("id", string.Empty));
            modal.AppendLine("    </m:modal.footer>");
            modal.Append("</m:modal>");

            obj.Elements.Render(brush, modal.ToString());
        }

        private string GetCropperPara(HtmlNode node, string paramName, string defaultValue)
        {
            var cropperNode = node.SelectSingleNodeEx("core/cropper");
            if (cropperNode == null) return defaultValue;
            return cropperNode.GetAttributeValue(paramName, defaultValue);
        }

        private void FillUploadHtml(SealedControl obj, HtmlNode node, PageBrush brush)
        {
            if (node.SelectSingleNodeEx("core/upload") == null) return;
            string modalId = string.Format("selectCoverModal_upload_{0}", node.GetAttributeValue("id", string.Empty));

            StringBuilder modal = new StringBuilder();

            modal.AppendFormat("<m:modal id=\"{0}\" size=\"full\" title=\"选择封面文件\">", modalId);
            modal.AppendLine();
            modal.AppendLine("        <bootform type=\"horizontal\" class=\"form-row-seperated thumbnails-form\">");
            modal.AppendFormat("            <m:upload name=\"upload\" class=\"thumbnails-input-cover\" max=\"1\" maxfilesize=\"{0}\" disk=\"true\" target=\"{1}\" extensions=\"(\\.|\\/)(gif|jpe?g|png)$\"  id=\"upload_{2}\">",
            //modal.AppendFormat("            <ms:input class=\"thumbnails-input-cover\" type=\"upload\" max=\"1\" maxfilesize=\"{0}\" target=\"{1}\" extensions=\"(\\.|\\/)(gif|jpe?g|png)$\"  id=\"upload_{2}\">",
                                    GetUploadPara(node, "maxfilesize", "40960"), GetUploadPara(node, "target", string.Empty), node.GetAttributeValue("id", string.Empty));
            modal.AppendLine();
            modal.AppendLine("                <m:inputLabel xs=\"12\" sm=\"3\">图片文件</m:inputLabel>");
            modal.AppendLine("                <m:inputCore xs=\"12\" sm=\"8\"></m:inputCore>");
            modal.AppendLine("                <m:inputHelp sm=\"8\" sm-offset=\"3\"></m:inputHelp>");
            modal.AppendLine("            </m:upload>");
            modal.AppendLine("        </bootform>");
            modal.AppendLine("    <m:modal.footer style=\"border-top:none;\">");
            modal.AppendLine("        <m:button class=\"thumbnails-upload-btn-ok\" color=\"blue\" icon=\"check\">确定</m:button>");
            modal.AppendLine("        <m:button color=\"default\" data-dismiss=\"modal\">取消</m:button>");
            modal.AppendFormat("        <m:alert id=\"thumbnailsAlert_upload_{0}\" style=\"margin-top:20px;\"></m:alert>", node.GetAttributeValue("id", string.Empty));
            modal.AppendLine();
            modal.AppendLine("    </m:modal.footer>");
            modal.Append("</m:modal>");

            obj.Elements.Render(brush, modal.ToString());

            //brush.DrawFormat("<metro:modal id=\"{0}\" width=\"80%\" title=\"选择封面文件\">", modalId);
            //brush.DrawLine();
            //brush.DrawLine("    <body class=\"form\">");
            //brush.DrawLine("        <boot:form type=\"horizontal\" class=\"form-row-seperated thumbnails-form\">");
            //brush.DrawFormat("            <metro:input class=\"thumbnails-input-cover\" type=\"upload\" max=\"1\" maxfilesize=\"{0}\" disk=\"true\" target=\"{1}\" extensions=\"(\\.|\\/)(gif|jpe?g|png)$\"  id=\"upload_{2}\">",
            //                            GetUploadPara(node, "maxfilesize", "40960"), GetUploadPara(node, "target", string.Empty), node.GetAttributeValue("id", string.Empty));
            //brush.DrawLine();
            //brush.DrawLine("                <label xs=\"12\" sm=\"3\">图片文件</label>");
            //brush.DrawLine("                <core xs=\"12\" sm=\"8\"></core>");
            //brush.DrawLine("                <help sm=\"8\" sm-offset=\"3\"></help>");
            //brush.DrawLine("            </metro:input>");
            //brush.DrawLine("        </boot:form>");
            //brush.DrawLine("    </body>");
            //brush.DrawLine("    <footer style=\"border-top:none;\">");
            //brush.DrawLine("        <metro:button class=\"thumbnails-upload-btn-ok\" color=\"blue\" icon=\"check\">确定</metro:button>");
            //brush.DrawLine("        <metro:button color=\"default\" data-dismiss=\"modal\">取消</metro:button>");
            //brush.DrawFormat("        <metro:alert id=\"thumbnailsAlert_upload_{0}\" style=\"margin-top:20px;\"></metro:alert>", node.GetAttributeValue("id", string.Empty));
            //brush.DrawLine();
            //brush.DrawLine("    </footer>");
            //brush.Draw("</metro:modal>");
        }

        private string GetUploadPara(HtmlNode node, string paramName, string defaultValue)
        {
            var uploadNode = node.SelectSingleNodeEx("core/upload");
            if (uploadNode == null) return defaultValue;
            return uploadNode.GetAttributeValue(paramName, defaultValue);
        }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            const string create = "$$.component.input.createThumbnails()";
            return UIUtil.GetProxyCode(obj as UIElement, node, create
                                           , UIUtil.GetJSONMembers(node, "required", "validate", "format:", "formatMessage:", "pulsate", "call:")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        private string GetLabel(HtmlNode node)
        {
            var labelNode = node.SelectSingleNodeEx("label");
            return labelNode == null ? string.Empty : labelNode.InnerText;
        }

        private string GetHelp(HtmlNode node)
        {
            var helpNode = node.SelectSingleNodeEx("help");
            return helpNode == null ? string.Empty : helpNode.InnerText;
        }

        protected override void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {

        }

        public static readonly InputThumbnailsPainter Instance = new InputThumbnailsPainter();
    }
}
