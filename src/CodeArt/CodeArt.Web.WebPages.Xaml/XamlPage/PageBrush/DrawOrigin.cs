using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    [TypeConverter(typeof(DrawOriginTypeConverter))]
    public enum DrawOrigin : byte
    {
        /// <summary>
        /// 页面的header区域
        /// </summary>
        Header = 0,
        Current = 1,
        /// <summary>
        /// 页面的底部，即&lt;/html&gt;的之前
        /// </summary>
        Bottom = 2
    }
}
