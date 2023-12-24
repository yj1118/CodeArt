
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.HtmlWrapper;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Sealed;
using HtmlAgilityPack;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputTextPainter : InputBasePainter
    {
        public InputTextPainter() { }

        protected override bool IsSetCoreSuffix(HtmlNode node, HtmlNode coreNode)
        {
            var temp = coreNode.SelectSingleNodeEx("before") ?? coreNode.SelectSingleNodeEx("after");
            return temp != null;
        }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            string create = string.Format("$$.component.input.create{0}()", node.GetAttributeValue("type", string.Empty).FirstToUpper());

            return UIUtil.GetProxyCode(obj as UIElement, node, create
                                           , UIUtil.GetJSONMembers(node, "required", "min", "max", "validate", "format:", "formatMessage:", "pulsate", "call:","xaml:true","name:","view:")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        protected override void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            FillSuffix(obj, coreNode.SelectSingleNodeEx("before"), brush);
            FillControl(obj, node, coreNode, brush);
            FillSuffix(obj, coreNode.SelectSingleNodeEx("after"), brush);
        }

        protected virtual void FillControl(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            string id = node.GetAttributeValue("id", string.Empty);
            string idCode = string.IsNullOrEmpty(id) ? string.Empty : string.Format(" id=\"control_{0}\"", id);
            string placeholder = node.GetAttributeValue("placeholder", string.Empty);

            StringBuilder className = new StringBuilder("form-control");
            var align = coreNode.GetAttributeValue("align", "left");
            if (align != "left") className.AppendFormat(" text-{0}", align);

            string status = string.Empty;
            string disabled = node.GetAttributeValue("disabled", "false");
            if (disabled == "true") status = " disabled";
            else
            {
                string readOnly = node.GetAttributeValue("readonly", "false");
                if (readOnly == "true") status = " readonly";
            }

            string propertyText = string.Format("class=\"{0}\"{1} data-name=\"input\" placeholder=\"{2}\"{3}", className, idCode, placeholder, status);
            brush.DrawLine(CreateControlCode(node, coreNode, propertyText));
        }

        protected virtual string CreateControlCode(HtmlNode node, HtmlNode coreNode, string propertyText)
        {
            string type = GetControlType(node);
            return string.Format("<input type=\"{0}\" {1} />", type, propertyText);
        }

        protected virtual string GetControlType(HtmlNode node)
        {
            return node.GetAttributeValue("type", "text");
        }

        protected void FillSuffix(SealedControl obj, HtmlNode suffixNode, PageBrush brush)
        {
            if (suffixNode == null) return;
            StringBuilder code = new StringBuilder();
            if (suffixNode.SelectNodesEx("metro:button").Count > 0 || suffixNode.SelectNodesEx("button").Count > 0)
            {
                code.AppendFormat("<span class=\"input-group-btn\">{0}</span>", suffixNode.InnerHtml);
            }
            else if (!string.IsNullOrEmpty(suffixNode.InnerText) && suffixNode.InnerText == suffixNode.InnerHtml)
            {
                //纯文本
                code.AppendFormat("<span class=\"input-group-addon\">{0}</span>", suffixNode.InnerText);
            }
            code.AppendLine();
            obj.Elements.Render(brush, code.ToString());
        }

        public static readonly InputTextPainter Instance = new InputTextPainter();
    }
}
