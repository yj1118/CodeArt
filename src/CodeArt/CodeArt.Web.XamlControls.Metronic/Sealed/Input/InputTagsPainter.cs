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
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputTagsPainter : InputBasePainter
    {
        public InputTagsPainter() { }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement, node, "$$.wrapper.metronic.input.createTags()"
                                           , UIUtil.GetJSONMembers(node, "required", "validate", "stringMode", "items::[]", "pulsate", "call:", "onchange", "onselect", "multiple")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        protected override void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            var name = node.GetAttributeValue("name", string.Empty);
            var select = node.GetAttributeValue("select", string.Empty);

            brush.DrawLine("<table class=\"input-tags-table\">");
            brush.DrawLine("<tr>");
            brush.DrawLine("<td class=\"input-tags-container\">");
            FillControl(node, coreNode, brush);
            brush.DrawLine("</td>");
            brush.DrawLine("<td class=\"input-tags-btnContainer\">");

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(select))
            {
                var invokeCode = ProxyCodeExtend.GetScriptEventCode(obj, node, "select:click");
                brush.DrawLine("<button class=\"btn blue\" data-name=\"select\" data-proxy=\"{" + invokeCode + "}\"><i class=\"fa fa-plus\"></i> 选择</button>");
            }
            else
            {
                obj.Elements.Render(brush, "<m:button data-name=\"select\" color=\"blue\"><m:Icon src=\"plus\"></m:Icon>选择</m:button>");
            }
            brush.DrawLine("</td>");
            brush.DrawLine("<td class=\"input-tags-btnContainer\">");
            obj.Elements.Render(brush, "<m:button data-name=\"reset\" color=\"purple\"><m:Icon src=\"times\"></m:Icon>清除</m:button>");
            brush.DrawLine("</td>");
            brush.DrawLine("</tr>");
            brush.DrawLine("</table>");
        }

        protected virtual void FillControl(HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            string placeholder = node.GetAttributeValue("placeholder", string.Empty);
            string multiple = node.GetAttributeValue("multiple", "false");
            brush.DrawFormat("<select class=\"form-control\" data-placeholder=\"{0}\"{1}>", placeholder, (multiple == "true" ? " multiple" : string.Empty));
            brush.DrawLine();
            brush.DrawLine("<option data-name='option' data-proxy=\"{loops:'items',display:'',binds:{'value':'value','text':'text'}}\"></option>");
            brush.Draw("</select>");
        }

        protected override string GetCoreContainerProxyCode(HtmlNode node)
        {
            return "{give:new $$.component.databind()}";
        }

        public static readonly InputTagsPainter Instance = new InputTagsPainter();
    }
}
