using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml;
using HtmlAgilityPack;

namespace CodeArt.Web.XamlControls.Metronic.Sealed
{
    internal class InputPasswordPainter : InputTextPainter
    {
        public InputPasswordPainter() { }

        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            string create = string.Format("$$.component.input.create{0}()", node.GetAttributeValue("type", string.Empty).FirstToUpper());

            return UIUtil.GetProxyCode(obj as UIElement, node, create
                                           , UIUtil.GetJSONMembers(node, "required", "min", "max", "validate", "format:", "target:", "formatMessage:")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        public new static readonly InputPasswordPainter Instance = new InputPasswordPainter();
    }
}
