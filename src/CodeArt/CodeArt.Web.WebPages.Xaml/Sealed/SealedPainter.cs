using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml;

namespace CodeArt.Web.WebPages.Xaml.Sealed
{
    public static class SealedPainter
    {
        public static string GetStyleCode(HtmlNode node)
        {
            string styleCode = node.GetAttributeValue("style", string.Empty);
            return string.IsNullOrEmpty(styleCode) ? string.Empty : string.Format(" style=\"{0}\"", styleCode);
        }

        public static void CreateNodeCode(PageBrush brush, string tagName, string className, string styleCode, string proxyCode, Action<PageBrush> fill, Func<string> getExtraCode = null)
        {
            if (!string.IsNullOrEmpty(proxyCode)) proxyCode = string.Format(" data-proxy=\"{0}\"", proxyCode);
            string extraCode = getExtraCode == null ? string.Empty : string.Format(" {0}", getExtraCode());

            brush.Draw(string.Format("<{0} class=\"{1}\"{2}{3}{4}>", tagName, className, styleCode, proxyCode, extraCode));
            brush.DrawLine();
            fill(brush);
            brush.DrawLine();
            brush.Draw(string.Format("</{0}>", tagName));
        }

        public static string GetFullClassName(HtmlNode node)
        {
            string className = node.GetAttributeValue("fullClass", string.Empty);
            return string.IsNullOrEmpty(className) ? string.Empty : string.Format(" class=\"{0}\"", className);
        }

    }
}
