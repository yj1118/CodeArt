using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.XamlControls.Bootstrap;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]

    public class InputHelpLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var element = obj as InputHelp;
            if (element == null) return;

            element.Class = GetClassName(objNode);
        }

        private string GetClassName(HtmlNode objNode)
        {
            var defaultClassName = LayoutUtil.GetClassName(objNode, "help-block");
            return UIUtil.GetClassName(objNode, defaultClassName);
        }

        

    }
}
