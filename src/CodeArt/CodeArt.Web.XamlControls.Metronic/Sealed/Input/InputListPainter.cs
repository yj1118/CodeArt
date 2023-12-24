using CodeArt.HtmlWrapper;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Sealed;
using CodeArt.Web.XamlControls.Bootstrap;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputListPainter : InputBasePainter
    {
        public InputListPainter() { }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement, node, "$$.wrapper.metronic.input.createList()"
                                           , UIUtil.GetJSONMembers(node, "required", "min", "max", "validate", "length", "oncreate", "onchange", "onset", "pulsate", "call:")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''", "closeProxy::true"));
        }

        protected override void FillLabel(HtmlNode node, PageBrush brush)
        {
            var labelNode = node.SelectSingleNodeEx("label");
            if (labelNode == null)
            {
                brush.Draw("<label class=\"control-label\" data-name=\"label\">");
                if (ShowOperation(node))
                {
                    brush.Draw("<button class=\"btn btn-sm green\" data-name=\"addItem\"><i class=\"fa fa-plus\"></i>新增项</button>");
                }
                brush.Draw("</label>");
                return;
            }

            string className = LayoutUtil.GetClassName(labelNode, "control-label");
            brush.DrawFormat("<label class=\"{0}\" data-name=\"label\">{1}", UIUtil.GetClassName(labelNode, className), labelNode.InnerText);
            if (ShowOperation(node))
            {
                brush.Draw(" &nbsp;<button class=\"btn btn-sm green\" data-name=\"addItem\"><i class=\"fa fa-plus\"></i>新增项</button>");
            }
            brush.Draw("</label>");
            brush.DrawLine();
        }

        private static bool ShowOperation(HtmlNode node)
        {
            return node.GetAttributeValue("operation", "true") != "false";
        }

        protected override void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            StringBuilder code = new StringBuilder();
            code.Append("<table class=\"table table-bordered table-striped table-input-list\"");
            code.Append("   xmlns:boot=\"http://schemas.codeart.cn/web/xaml\"");
            code.Append("   xmlns:metro=\"http://schemas.codeart.cn/web/xaml/metronic\"");
            code.AppendLine(">");
            code.AppendLine(CreateHeadCode(node));
            code.AppendLine("<tbody>");
            code.AppendLine(CreateBodyCode(node));
            code.AppendLine("</tbody>");
            code.AppendLine("</table>");
            obj.Elements.Render(brush, code.ToString());
        }

        private static string CreateHeadCode(HtmlNode node)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<thead><tr>");

            var items = node.SelectNodesEx("core/headers/item");
            foreach (var item in items)
            {
                var name = item.InnerText;
                html.AppendFormat("<th {0}{1}>{2}</th>", SealedPainter.GetFullClassName(item), SealedPainter.GetStyleCode(item), name);
            }
            if (ShowOperation(node))
            {
                html.AppendLine("<th class=\"text-center\">操作</th>");
            }
            html.Append("</tr></thead>");
            return html.ToString();
        }

        private static string CreateBodyCode(HtmlNode node)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<tr data-name=\"listItem\">");
            var items = node.SelectNodesEx("core/columns/item");
            foreach (var item in items)
            {
                var colspan = item.GetAttributeValue("colspan", string.Empty);
                if (colspan.Length == 0) html.AppendLine("<td>");
                else html.AppendFormat("<td colspan=\"{0}\">", colspan);
                html.Append(item.InnerHtml);
                html.AppendLine("</td>");
            }
            if (ShowOperation(node))
            {
                html.AppendLine("<td class=\"table-input-list-operation\">");
                html.AppendLine("<boot:container type=\"fluid\">");
                html.AppendLine("<boot:row>");
                html.AppendLine("<boot:column xs=\"12\" sm=\"6\" md=\"3\" class=\"col-trim text-center\">");
                html.AppendLine("<metro:button data-name=\"prevItem\" color=\"blue\" size=\"sm\">上移</metro:button>");
                html.AppendLine("</boot:column>");
                html.AppendLine("<boot:column xs=\"12\" sm=\"6\" md=\"3\" class=\"col-trim text-center\">");
                html.AppendLine("<metro:button data-name=\"nextItem\" color=\"blue\" size=\"sm\">下移</metro:button>");
                html.AppendLine("</boot:column>");
                html.AppendLine("<boot:column xs=\"12\" sm=\"6\" md=\"3\" class=\"col-trim text-center\">");
                html.AppendLine("<metro:button data-name=\"resetItem\" color=\"yellow\" size=\"sm\">清空</metro:button>");
                html.AppendLine("</boot:column>");
                html.AppendLine("<boot:column xs=\"12\" sm=\"6\" md=\"3\" class=\"col-trim text-center\">");
                html.AppendLine("<metro:button data-name=\"removeItem\" color=\"red\" size=\"sm\">移除</metro:button>");
                html.AppendLine("</boot:column>");
                html.AppendLine("</td>");
            }
            html.AppendLine("</tr>");
            return html.ToString();
        }

        public static readonly InputListPainter Instance = new InputListPainter();
    }
}
