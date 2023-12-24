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
    internal class InputTextsPainter : InputBasePainter
    {
        public InputTextsPainter() { }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement, node, "$$.wrapper.metronic.input.createTexts()"
                                           , UIUtil.GetJSONMembers(node, "required", "validate", "pulsate", "call:", "length", "min")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        protected override void FillCoreContainer(SealedControl obj, HtmlNode node, PageBrush brush)
        {
            var hasBrowseNode = node.SelectSingleNodeEx("browse") != null;
            var coreNode = node.SelectSingleNodeEx("core");
            if (coreNode == null) throw new WebException("没有指定core节点");
            string className = LayoutUtil.GetClassName(coreNode, "input-extend");
            string styleCode = hasBrowseNode ? "display:none;" : string.Empty;

            if (!string.IsNullOrEmpty(styleCode)) styleCode = string.Format(" style=\"{0}\"", styleCode);
            SealedPainter.CreateNodeCode(brush, "div", className, styleCode, GetCoreContainerProxyCode(node), (pageBrush) =>
            {
                FillCore(obj, node, coreNode, pageBrush);
            },
            () =>
            {
                return "data-name='coreContainer'";
            });
            //html.AppendLine(code);
        }

        protected override void FillCore(SealedControl obj, HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            FillControl(node, coreNode, brush);
        }

        protected virtual void FillControl(HtmlNode node, HtmlNode coreNode, PageBrush brush)
        {
            brush.DrawLine("<div class=\"col-lg-2 col-md-3 col-sm-4 col-xs-6 input-texts-item\" data-proxy=\"{loops:'items',display:''}\">");
            brush.DrawLine(CreateControlCode(node));
            brush.Draw("</div>");
        }

        protected virtual string CreateControlCode(HtmlNode node)
        {
            string status = string.Empty;
            string disabled = node.GetAttributeValue("disabled", "false");
            if (disabled == "true") status = " disabled";
            else
            {
                string readOnly = node.GetAttributeValue("readonly", "false");
                if (readOnly == "true") status = " readonly";
            }

            return string.Format("<input type=\"text\" class=\"form-control\" data-name=\"input\" {0} />", status);
        }

        protected override string GetCoreContainerProxyCode(HtmlNode node)
        {
            return "{give:new $$.component.databind()}";
        }

        public static readonly InputTextsPainter Instance = new InputTextsPainter();
    }
}
