using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.HtmlWrapper;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [SafeAccess]

    public class HtmlElementLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            var element = obj as HtmlElement;
            element.TagName = objNode.OriginalName;
            element.IsSingNode = objNode.IsSingleNode();
            base.Load(obj, objNode);
        }
    }
}
