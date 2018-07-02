using CodeArt.HtmlWrapper;

using CodeArt.Web.WebPages.Xaml;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputDatePainter : InputTextPainter
    {
        public InputDatePainter() { }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement, node, "$$.wrapper.metronic.input.createDate()"
                                           , UIUtil.GetJSONMembers(node, "required", "mode:", "validate", "format:", "formatMessage:", "pulsate", "call:")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        protected override string CreateControlCode(HtmlNode node, HtmlNode coreNode, string propertyText)
        {
            return string.Format("<input type=\"text\" {0} />", propertyText);
        }

        public new static readonly InputDatePainter Instance = new InputDatePainter();
    }
}
