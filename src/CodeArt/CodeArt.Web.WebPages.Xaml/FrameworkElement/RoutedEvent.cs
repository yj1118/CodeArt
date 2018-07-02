using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public enum RoutedEvent : byte
    {
        /// <summary>
        /// 组件初始化完毕
        /// </summary>
        Init = 1,
        /// <summary>
        /// 组件加载完毕
        /// </summary>
        Load = 2,
        /// <summary>
        /// 组件渲染之前
        /// </summary>
        PreRender = 3
    }
}
