using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Sealed
{
    [SafeAccess]

    public class SealedControlLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            base.Load(obj, objNode);
            var c = obj as SealedControl;
            if (c != null) c.SourceCode = objNode.OuterHtml;
        }
    }
}
