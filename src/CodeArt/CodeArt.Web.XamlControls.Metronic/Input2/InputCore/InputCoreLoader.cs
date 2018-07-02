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
    public class InputCoreLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);

            var core = obj as InputCore;
            if (core != null)
            {
                var isSetCoreSuffix = IsSetCoreSuffix(objNode);
                var hasBrowseNode = HasBrowseNode(objNode);
                core.Class = GetClassName(objNode, isSetCoreSuffix);
                core.Style = GetStyleCode(objNode, isSetCoreSuffix, hasBrowseNode);
            }
        }

        private string GetClassName(HtmlNode node, bool isSetCoreSuffix)
        {
            var defaultClassName = isSetCoreSuffix
                    ? LayoutUtil.GetClassName(node, "input-group input-extend")
                    : LayoutUtil.GetClassName(node, "input-extend");

            return UIUtil.GetClassName(node, defaultClassName);
        }

        private string GetStyleCode(HtmlNode node, bool isSetCoreSuffix, bool hasBrowseNode)
        {
            string styleCode = hasBrowseNode ? "display:none;" : string.Empty;
            if (isSetCoreSuffix) styleCode += "padding-left:15px; padding-right:15px;";
            return styleCode;
        }

        private bool IsSetCoreSuffix(HtmlNode node)
        {
            var childNodes = node.ChildElements();
            foreach(var child in childNodes)
            {
                var nodeName = child.LocalOriginalName();
                if (nodeName.Equals("inputCore.After", StringComparison.OrdinalIgnoreCase)
                    || nodeName.Equals("inputCore.Before", StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        private bool HasBrowseNode(HtmlNode node)
        {
            var parent = node.ParentNode;
            var childNodes = parent.ChildElements();
            foreach (var child in childNodes)
            {
                var nodeName = child.LocalOriginalName();
                if (nodeName.Equals("InputBrowse", StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public new static readonly InputCoreLoader Instance = new InputCoreLoader();

    }
}
