using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.XamlControls.Bootstrap;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class TreeLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            var tree = obj as Tree;
            if (tree == null) return;

            tree.ProxyCode = this.GetProxyCode(obj, objNode);

            base.Load(obj, objNode);
        }

        private string GetProxyCode(object obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement, node,
                                        "$$metronic.tree.create()",
                                        UIUtil.GetJSONMembers(node, "name:", "view:"),
                                        UIUtil.GetJSONMembers(node, "id:", "name:"));
        }
    }
}
