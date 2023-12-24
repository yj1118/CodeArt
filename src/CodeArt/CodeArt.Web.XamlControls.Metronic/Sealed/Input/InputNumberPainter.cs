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
    internal class InputNumberPainter : InputTextPainter
    {
        public InputNumberPainter() { }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement,node, "$$.component.input.createNumber()"
                                           , UIUtil.GetJSONMembers(node, "required", "minValue", "maxValue", "validate", "mode::'float'", "format:", "formatMessage:", "pulsate", "call:")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        public new static readonly InputNumberPainter Instance = new InputNumberPainter();
    }
}
