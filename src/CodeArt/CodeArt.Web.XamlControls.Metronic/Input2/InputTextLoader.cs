using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.XamlControls.Bootstrap;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class InputTextLoader : InputLoader
    {
        protected override string GetProxyCode(object obj, HtmlNode node)
        {
            string create = string.Format("$$.component.input.create{0}()", GetType(node));

            return UIUtil.GetProxyCode(obj as UIElement, node, create
                                           , UIUtil.GetJSONMembers(node, "required", "min", "max", "validate", "format:", "formatMessage:", "pulsate", "call:")
                                           , UIUtil.GetJSONMembers(node, "id:", "name:", "form::''"));
        }

        protected virtual string GetType(HtmlNode node)
        {
            return node.GetAttributeValue("type", string.Empty).FirstToUpper();
        }


        public new static readonly InputTextLoader Instance = new InputTextLoader();

    }
}
