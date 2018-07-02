using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Runtime;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    internal static class ProxyCodeCombine
    {
        
        public static string CombineCode(UIElement e, HtmlNode objNode)
        {
            var proxyCode = e.ProxyCode;
            var nodeProxyCode = objNode.GetAttributeValue("data-proxy", string.Empty);

            if (string.IsNullOrEmpty(nodeProxyCode)) return proxyCode;

            var nodeProxyCodeValue = nodeProxyCode.Substring(1, nodeProxyCode.Length - 2);


            if (string.IsNullOrEmpty(proxyCode))
            {
                proxyCode = "{" + nodeProxyCodeValue + "}";
                return proxyCode;
            }

            //此处没有考虑重复代码
            proxyCode = proxyCode.Insert(proxyCode.Length - 1, "," + nodeProxyCodeValue);
            return proxyCode;
        }


    }
}