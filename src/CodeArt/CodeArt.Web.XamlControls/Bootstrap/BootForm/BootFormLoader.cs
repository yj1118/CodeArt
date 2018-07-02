using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using HtmlAgilityPack;

namespace CodeArt.Web.XamlControls
{
    public class BootFormLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var form = obj as BootForm;
            if (form != null)
            {
                form.ProxyCode = GetProxyCode(obj, objNode);
                form.Class = UIUtil.GetClassName(objNode, GetTypeClassName(objNode));
            }
        }

        private string GetTypeClassName(HtmlNode node)
        {
            string type = node.GetAttributeValue("type", string.Empty);
            if (string.IsNullOrEmpty(type)) type = "horizontal"; //如果没有指定，那么默认为垂直模式
            switch (type)
            {
                case "horizontal": return "form-horizontal";
                case "inline": return "form-inline";
            }
            return "form";
        }

        private string GetProxyCode(object obj, HtmlNode node)
        {
            return UIUtil.GetProxyCode(obj as UIElement, node, "new $$.component.form()", UIUtil.GetJSONMembers(node, "validate"), UIUtil.GetJSONMembers(node, "id:", "name:"));
        }
    }
}
