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
    public abstract class InputLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var input = obj as Input;
            if (input != null)
            {
                input.Class = GetClassName(objNode);
                input.ProxyCode = GetProxyCode(obj,objNode);
            }
        }

        protected virtual string GetClassName(HtmlNode node)
        {
            var defaultClassName = string.Format("input input-{0} form-group", node.GetAttributeValue("type", string.Empty));
            return UIUtil.GetClassName(node, defaultClassName);
        }

        protected abstract string GetProxyCode(object obj, HtmlNode node);

    }
}
