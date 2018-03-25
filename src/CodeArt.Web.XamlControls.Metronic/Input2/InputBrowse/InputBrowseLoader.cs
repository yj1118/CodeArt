using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.XamlControls.Bootstrap;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]

    public class InputBrowseLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var element = obj as InputHelp;
            if (element == null) return;

            element.Class = GetClassName(objNode);
            element.ProxyCode = GetProxyCode(objNode);
        }

        private string GetClassName(HtmlNode objNode)
        {
            string className = LayoutUtil.GetClassName(objNode, "input-browse");
            return UIUtil.GetClassName(objNode, className);
        }

        private string GetProxyCode(HtmlNode objNode)
        {
            return UIUtil.GetJSONMembers(objNode, "only");
        }
    }
}
