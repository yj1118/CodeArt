using CodeArt.HtmlWrapper;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Sealed;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputUploadPainter : InputBasePainter
    {
        public InputUploadPainter() { }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            var target = node.GetAttributeValue("target", string.Empty);
            if (string.IsNullOrEmpty(target)) node.SetAttributeValue("getTarget", this.GetDiskGetTarget());

            var assetsPath = node.GetAttributeValue("assetsPath", string.Empty);
            if (string.IsNullOrEmpty(assetsPath)) node.SetAttributeValue("assetsPath", this.GetAssetsPath());

            return UIUtil.GetProxyCode(obj as UIElement, node, "$$.wrapper.metronic.input.createUpload()"
                                           , UIUtil.GetJSONMembers(node, "required", "min", "max", "validate", "extensions:", "pulsate", "call:", "disk", "maxFileSize", "target:", "getTarget:", "folderId:", "assetsPath:")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        private string GetDiskGetTarget()
        {
            var target = ConfigurationManager.AppSettings["metronic:diskGetTarget"];
            if (string.IsNullOrEmpty(target)) throw new WebException("没有配置metronic:diskGetTarget");
            return target;
        }

        private string GetAssetsPath()
        {
            var assetsPath = ConfigurationManager.AppSettings["metronic:diskAssetsPath"];
            if (string.IsNullOrEmpty(assetsPath)) throw new WebException("没有配置metronic:diskAssetsPath");
            if (assetsPath.EndsWith("/")) assetsPath = assetsPath.Substring(0, assetsPath.Length - 1);
            return assetsPath;
        }

        protected override void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            UIUtil.CheckProperties(node, "id");

            brush.DrawFormat("<form data-name=\"edit-files\" class=\"edit-files\" action=\"{0}\" method=\"POST\" enctype=\"multipart/form-data\">", node.GetAttributeValue("target", "#"));
            brush.DrawLine("<input type=\"hidden\" name=\"folderId\">");
            brush.DrawLine("	<div class=\"row fileupload-buttonbar\">");
            brush.DrawLine("		<div class=\"col-lg-7\">");
            if (node.GetAttributeValue("localUpload", "true") == "true")
            {
                brush.DrawLine("			<span class=\"btn btn-sm green fileinput-button\">");
                brush.DrawLine("			<i class=\"fa fa-plus\"></i>");
                brush.DrawLine("			<span>选择文件... </span>");
                brush.DrawLine("			<input type=\"file\" name=\"files[]\" multiple=\"\">");
                brush.DrawLine("			</span>");
                brush.DrawLine("			<button type=\"submit\" class=\"btn btn-sm blue start\">");
                brush.DrawLine("			<i class=\"fa fa-upload\"></i>");
                brush.DrawLine("			<span>开始上传 </span>");
                brush.DrawLine("			</button>");
                brush.DrawLine("			<button type=\"reset\" class=\"btn btn-sm red cancel\">");
                brush.DrawLine("			<i class=\"fa fa-ban\"></i>");
                brush.DrawLine("			<span>移除文件 </span>");
                brush.DrawLine("			</button>");
            }
            if (node.GetAttributeValue("disk", "false") == "true")
            {
                brush.DrawLine("			<button type=\"button\" class=\"btn btn-sm purple fileinput-disk\">");
                brush.DrawLine("			<i class=\"fa fa-folder-open\"></i>");
                brush.DrawLine("			<span>从文件空间中选择 </span>");
                brush.DrawLine("			</button>");
            }

            brush.DrawLine("			<span class=\"fileupload-process\">");
            brush.DrawLine("			</span>");
            brush.DrawLine("		</div>");
            brush.DrawLine("		<div class=\"col-lg-5 fileupload-progress fade\">");
            brush.DrawLine("			<div class=\"progress progress-striped active\" role=\"progressbar\" aria-valuemin=\"0\" aria-valuemax=\"100\">");
            brush.DrawLine("				<div class=\"progress-bar progress-bar-success\" style=\"width:0%;\">");
            brush.DrawLine("				</div>");
            brush.DrawLine("			</div>");
            brush.DrawLine("			<div class=\"progress-extended\">&nbsp;</div>");
            brush.DrawLine("		</div>");
            brush.DrawLine("	</div>");
            brush.DrawLine("	<table role=\"presentation\" class=\"table table-striped clearfix\">");
            brush.DrawLine("	<tbody class=\"files\">");
            brush.DrawLine("	</tbody>");
            brush.DrawLine("	</table>");
            brush.DrawLine("</form>");
            if (node.GetAttributeValue("disk", "false") == "true")
            {
                StringBuilder modal = new StringBuilder();

                modal.AppendLine("<m:modal data-name=\"disk-modal\" size=\"full\" title=\"请选择文件\">");
                modal.AppendLine("<m:disk id=\"" + node.GetAttributeValue("id", string.Empty) + "\" data-name=\"disk\" name=\"disk\" height=\"500px\"></m:disk>");
                modal.AppendLine("<m:modal.footer>");
                modal.AppendLine("<m:button color=\"default\" data-dismiss=\"modal\">取消关闭</m:button>");
                modal.AppendLine("</m:modal.footer>");
                modal.AppendLine("</m:modal>");

                obj.Elements.Render(brush, modal.ToString());
            }
        }

        public static readonly InputUploadPainter Instance = new InputUploadPainter();


        #region 代码资产

        public static readonly RawCode CodeAssets = null;

        static InputUploadPainter()
        {
            CodeAssets = CreateCodeAssets();
        }

        private static RawCode CreateCodeAssets()
        {
            StringBuilder code = new StringBuilder();
            code.AppendLine("<div id=\"blueimp-gallery\" class=\"blueimp-gallery blueimp-gallery-controls\" data-filter=\":even\">");
            code.AppendLine("<div class=\"slides\"></div>");
            code.AppendLine("<h3 class=\"title\"></h3>");
            code.AppendLine("<a class=\"prev\">‹</a>");
            code.AppendLine("<a class=\"next\">›</a>");
            code.AppendLine("<a class=\"close\">×</a>");
            code.AppendLine("<a class=\"play-pause\"></a>");
            code.AppendLine("<ol class=\"indicator\"></ol>");
            code.AppendLine("</div>");

            code.AppendLine("<script id=\"template-upload\" type=\"text/x-tmpl\">");
            code.AppendLine("    {% for (var i=0, file; file=o.files[i]; i++) { %}");
            code.AppendLine("    <tr class=\"template-upload fade\">");
            code.AppendLine("        <td>");
            code.AppendLine("            <span class=\"preview\"></span>");
            code.AppendLine("        </td>");
            code.AppendLine("        <td>");
            code.AppendLine("            <p class=\"name\">{%=file.name%}</p>");
            code.AppendLine("            <strong class=\"error label label - danger\"></strong>");
            code.AppendLine("        </td>");
            code.AppendLine("        <td>");
            code.AppendLine("            <p class=\"size\">处理中...</p>");
            code.AppendLine("            <div class=\"progress progress - striped active\" role=\"progressbar\" aria-valuemin=\"0\" aria-valuemax=\"100\" aria-valuenow=\"0\">");
            code.AppendLine("                <div class=\"progress-bar progress-bar-success\" style=\"width:0%;\"></div>");
            code.AppendLine("            </div>");
            code.AppendLine("        </td>");
            code.AppendLine("        <td>");
            code.AppendLine("            {% if (!i && !o.options.autoUpload) { %}");
            code.AppendLine("            <button class=\"btn blue start btn-sm\" disabled>");
            code.AppendLine("                <i class=\"fa fa-upload\"></i>");
            code.AppendLine("                <span>上传</span>");
            code.AppendLine("            </button>");
            code.AppendLine("            {% } %}");
            code.AppendLine("            {% if (!i) { %}");
            code.AppendLine("            <button class=\"btn red cancel btn-sm\">");
            code.AppendLine("                <i class=\"fa fa-ban\"></i>");
            code.AppendLine("                <span>移除</span>");
            code.AppendLine("            </button>");
            code.AppendLine("            {% } %}");
            code.AppendLine("        </td>");
            code.AppendLine("    </tr>");
            code.AppendLine("    {% } %}");
            code.AppendLine("</script>");
            code.AppendLine("<script id=\"template-download\" type=\"text/x-tmpl\">");
            code.AppendLine("    {% for (var i=0, file; file=o.files[i]; i++) { %}");
            code.AppendLine("    <tr class=\"template-download fade\" data-id=\"{%=file.id%}\" data-size=\"{%=file.size%}\" data-name=\"{%=file.name%}\" data-url=\"{%=file.url%}\" data-thumbnailurl=\"{%=file.thumbnailUrl%}\" data-extension=\"{%=file.extension%}\" data-error=\"{%=file.error%}\">");
            code.AppendLine("        <td>");
            code.AppendLine("            <span class=\"preview\">");
            code.AppendLine("                {% if (file.thumbnailUrl) { %}");
            code.AppendLine("                <a href=\"{%=file.url%}\" title=\"{%=file.name%}\" download=\"{%=file.name%}\" data-gallery><img src=\"{%=file.thumbnailUrl%}\"></a>");
            code.AppendLine("                {% } else if(file.videoUrl){ %}");
            code.AppendLine("                <video src=\"{%=file.videoUrl%}\" controls=\"\"></video>");
            code.AppendLine("                {% }%}");
            code.AppendLine("            </span>");
            code.AppendLine("        </td>");
            code.AppendLine("        <td>");
            code.AppendLine("            <p class=\"name\">");
            code.AppendLine("                {% if (file.url) { %}");
            code.AppendLine("                <a href=\"{%=file.url%}\" title=\"{%=file.name%}\" download=\"{%=file.name%}\" >{%=file.name%}</a>");
            code.AppendLine("                {% } else { %}");
            code.AppendLine("                <span>{%=file.name%}</span>");
            code.AppendLine("                {% } %}");
            code.AppendLine("            </p>");
            code.AppendLine("            {% if (file.error) { %}");
            code.AppendLine("            <div><span class=\"label label-danger\">{%=file.error%}</span></div>");
            code.AppendLine("            {% } %}");
            code.AppendLine("        </td>");
            code.AppendLine("        <td>");
            code.AppendLine("            <span class=\"size\">{%=o.formatFileSize(file.size)%}</span>");
            code.AppendLine("        </td>");
            code.AppendLine("        <td>");
            code.AppendLine("            {% if (file.deleteUrl) { %}");
            code.AppendLine("            <button class=\"btn red delete btn-sm\" data-type=\"{%=file.deleteType%}\" data-url=\"{%=file.deleteUrl%}\" {% if (file.deletewithcredentials) { %} data-xhr-fields='{\"withCredentials\":true}' {% } %}>");
            code.AppendLine("                <i class=\"fa fa-trash-o\"></i>");
            code.AppendLine("                <span>删除</span>");
            code.AppendLine("            </button>");
            code.AppendLine("            <input type=\"checkbox\" name=\"delete\" value=\"1\" class=\"toggle\">");
            code.AppendLine("            {% } else { %}");
            code.AppendLine("            <button class=\"btn yellow cancel btn-sm\">");
            code.AppendLine("                <i class=\"fa fa-ban\"></i>");
            code.AppendLine("                <span>取消</span>");
            code.AppendLine("            </button>");
            code.AppendLine("            {% } %}");
            code.AppendLine("        </td>");
            code.AppendLine("    </tr>");
            code.AppendLine("    {% } %}");
            code.AppendLine("</script>");
            return new RawCode() { Code = code.ToString(), Origin = DrawOrigin.Bottom };
        }

        #endregion


    }
}
