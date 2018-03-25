using CodeArt.HtmlWrapper;
using CodeArt.Util;
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
    internal class InputDropdownPainter : InputBasePainter
    {
        public InputDropdownPainter() { }

        public override void FillHtml(object obj, HtmlNode node, PageBrush brush)
        {
            var group = node.GetAttributeValue("group", "false");
            if (group == "false")
            {
                if (node.SelectNodesEx("//group").Count > 0) group = "true"; //如果选项中已经有group，那么必须为true
            }
            node.SetAttributeValue("group", group);

            if (IsSingleLevel(node))
            {
                var emptyItem = node.GetAttributeValue("emptyItem", "true"); //默认允许有空项
                node.SetAttributeValue("emptyItem", emptyItem);
            }
            else
            {
                var emptyItem = node.GetAttributeValue("emptyItem", "false"); //默认不允许有空项
                node.SetAttributeValue("emptyItem", emptyItem);
            }

            node.SetAttributeValue("level", GetLevelCount(node).ToString());

            base.FillHtml(obj, node, brush);
        }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            StringBuilder uiParasCode = new StringBuilder(UIUtil.GetJSONMembers(node, "required", "validate", "onchange", 
                "onunselect", "multiple", "group", "stringMode", "pulsate", "call:", "level", 
                "loadOptions", "emptyItem", "autoHidden", "placeholder::''", "xaml:true",
                "name:","view:"));
            if (uiParasCode.Length > 0) uiParasCode.Append(",");
            uiParasCode.AppendFormat("items:{0}", UIUtil.GetOptionItems(node));

            return UIUtil.GetProxyCode(obj as UIElement, node, (IsSingleLevel(node) ? "$$.wrapper.metronic.input.createDropdown()" : "$$.wrapper.metronic.input.createDropdownLevel()")
                                           , uiParasCode.ToString()
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        protected override void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            if (IsSingleLevel(node)) FillControl(node, coreNode, brush);
            else FillControlByMultiLevel(obj,node, coreNode, brush);
        }

        private static int GetLevelCount(HtmlNode node)
        {
            return node.SelectNodesEx("core/unit").Count;
        }

        protected virtual void FillControl(HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            string placeholder = node.GetAttributeValue("placeholder", string.Empty);
            string multiple = node.GetAttributeValue("multiple", "false");
            var group = node.GetAttributeValue("group", "false");
            brush.DrawFormat("<select class=\"form-control\" data-placeholder=\"{0}\"{1}>", placeholder, (multiple == "true" ? " multiple" : string.Empty));
            brush.DrawLine();

            if (group == "false")
            {
                brush.DrawLine("<option data-name='option' data-proxy=\"{loops:'items',display:'',binds:{'value':'value','text':'text'}}\"></option>");
            }
            else
            {
                brush.DrawLine("<optgroup data-proxy=\"{loops:'groups',display:'',binds:{'label':'name'}}\">");
                brush.DrawLine("<option data-name='option' data-proxy=\"{loops:'items',display:'',binds:{'value':'value','text':'text'}}\"></option>");
                brush.DrawLine("</optgroup>");
            }
            brush.Draw("</select>");
        }

        protected virtual void FillControlByMultiLevel(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            var unitNodes = node.SelectNodesEx("core/unit");
            var hidden = node.GetAttributeValue("autoHidden", "true") == "true" ? "display:none;" : string.Empty;
            for (var i = 0; i < unitNodes.Count; i++)
            {
                var levelNode = unitNodes[i];
                string placeholder = levelNode.GetAttributeValue("placeholder", string.Empty);
                var group = levelNode.GetAttributeValue("group", "false");

                StringBuilder code = new StringBuilder();
                if (i + 1 >= unitNodes.Count)
                {
                    code.AppendFormat("<column {0} style=\"{1}padding-bottom:2px;\" class=\"col-trim\">", LayoutUtil.GetProperiesCode(levelNode), hidden);
                }
                else
                {
                    code.AppendFormat("<column {0} style=\"{1}padding-right:2px;padding-bottom:2px;\" class=\"col-trim\">", LayoutUtil.GetProperiesCode(levelNode), hidden);
                }
                code.AppendLine();
                code.AppendFormat("<ms:dropdown data-name=\"unit\" emptyItem=\"{0}\" validate=\"false\">", node.GetAttributeValue("emptyItem", "false"));
                code.AppendLine();
                code.AppendLine("<label remove=\"true\"></label>");
                code.AppendLine("<core xs=\"12\" class=\"col-trim\">");
                code.AppendLine("</core>");
                code.AppendLine("</ms:dropdown>");
                code.AppendLine("</column>");

                obj.Elements.Render(brush, code.ToString());
            }
        }

        protected override string GetCoreContainerProxyCode(HtmlNode node)
        {
            if (IsSingleLevel(node))
            {
                return "{give:new $$.component.databind()}";
            }
            return string.Empty;
        }

        public static bool IsSingleLevel(HtmlNode node)
        {
            return GetLevelCount(node) <= 1;
        }


        public static readonly InputDropdownPainter Instance = new InputDropdownPainter();
    }
}
