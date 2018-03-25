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
    internal class InputHiddenPainter : InputTextPainter
    {
        public InputHiddenPainter() { }

        public override void FillHtml(object obj, HtmlNode node, PageBrush brush)
        {
            node.SetAttributeValue("style", "display:none;");
            base.FillHtml(obj, node, brush);
        }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            string create = "$$.component.input.createText()";

            return UIUtil.GetProxyCode(obj as UIElement, node, create
                                           , UIUtil.GetJSONMembers(node, "required", "min", "max", "validate", "format:", "formatMessage:", "pulsate", "call:")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        protected override void FillLabel(HtmlNode node, PageBrush brush)
        {

        }

        protected override void FillHelp(HtmlNode node, PageBrush brush)
        {

        }

        protected override void FillCoreContainer(SealedControl obj, HtmlNode node, PageBrush brush)
        {
            string className = "input-extend";

            SealedPainter.CreateNodeCode(brush, "div", className, string.Empty, GetCoreContainerProxyCode(node), (pageBrush) =>
            {
                pageBrush.Draw("<input data-name=\"input\" type=\"hidden\" />");
            },
            () =>
            {
                return "data-name='coreContainer'";
            });
        }

        public new static readonly InputHiddenPainter Instance = new InputHiddenPainter();
    }
}
