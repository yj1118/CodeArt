using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace CodeArt.Web.WebPages.Xaml
{
    public interface IUIElement
    {
        /// <summary>
        /// 在web页面中呈现元素
        /// </summary>
        /// <param name="brush"></param>
        void Render(PageBrush brush);
    }
}
