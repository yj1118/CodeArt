using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;

using HtmlAgilityPack;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]
    public class RawCodeLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            var ac = obj as RawCode;
            ac.Code = objNode.InnerHtml;
            PropertiesLoader.Load(obj, objNode);
        }
    }
}
