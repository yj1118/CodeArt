using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml;

namespace CodeArt.Web.XamlControls.Bootstrap
{
    [SafeAccess]
    public class ContainerLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var element = obj as Container;
            if (element == null) return;

            element.Class = GetClassName(objNode);
        }

        private string GetClassName(HtmlNode objNode)
        {
            var type = objNode.GetAttributeValue("type", string.Empty);
            var fixClassName = type == "fluid" ? UIUtil.GetClassName(objNode, "container-fluid") : UIUtil.GetClassName(objNode, "container");
            fixClassName = UIUtil.GetClassName(objNode, fixClassName);

            return string.Format("{0}", fixClassName).Trim();
        }
    }
}
