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
    public class TabLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            var tab = obj as Tab;
            if (tab == null) return;

            UIUtil.CheckProperties(objNode, "id");
            tab.ProxyCode = this.GetProxyCode(obj, objNode);
            tab.Class = this.GetClassName(objNode);
            base.Load(obj, objNode);
        }

        private string GetProxyCode(object obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement, node, "new $$metronic.tab()", UIUtil.GetJSONMembers(node, "onselect"), string.Empty);
        }

        private string GetClassName(HtmlNode objNode)
        {
            var className = objNode.GetAttributeValue("class", string.Empty);
            string fixedClassName = "tab";

            if (string.IsNullOrEmpty(className)) className = "tab-default";

            return string.Format("{0} {1}", fixedClassName, className).Trim();
        }
    }
}
