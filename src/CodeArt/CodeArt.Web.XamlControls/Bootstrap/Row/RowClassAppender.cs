using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;


namespace CodeArt.Web.XamlControls.Bootstrap
{
    /// <summary>
    /// class样式附加器
    /// </summary>
    [SafeAccess]
    internal class RowClassAppender : ClassAppender
    {
        protected override string GetBaseClass(UIElement target, UIElement templateParent)
        {
            return "row";
        }

        public static readonly RowClassAppender Instance = new RowClassAppender();

    }
}