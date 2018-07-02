using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 将xaml定义的对象信息，加载到目标对象中
    /// </summary>
    internal interface IComponentLoader
    {
        void LoadComponent(object obj, HtmlNode objNode);
    }
}
