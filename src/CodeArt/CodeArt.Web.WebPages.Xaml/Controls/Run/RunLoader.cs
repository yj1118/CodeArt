using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [SafeAccess]

    public class RunLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            if (objNode.NodeType == HtmlNodeType.Text)
            {
                var text = obj as Run;
                text.Content = objNode.InnerText;
            }
            else
                base.Load(obj, objNode);
        }
    }
}
