using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;
using HtmlAgilityPack;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    [SafeAccess]

    public class CommentLoader : ComponentLoader
    {
        protected override void Load(object obj, HtmlNode objNode)
        {
            if (objNode.NodeType == HtmlNodeType.Comment)
            {
                var comment = obj as Comment;
                comment.Content = objNode.OuterHtml;
            }
            else
                base.Load(obj, objNode);
        }
    }
}
