using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.XamlControls.Bootstrap
{
    [SafeAccess]

    public class ColumnLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var element = obj as Column;
            if (element == null) return;

            element.Class = GetClassName(objNode);
        }

        private string GetClassName(HtmlNode objNode)
        {
            var className = objNode.GetAttributeValue("class", string.Empty);

            string fixedClassName = LayoutUtil.GetClassName(objNode, string.Empty);

            fixedClassName = string.Format("{0} {1}", fixedClassName, "col-extend").Trim();

            return string.Format("{0} {1}", fixedClassName, className).Trim();
        }
    }
}
