using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;

using CodeArt.DTO;

using CodeArt.Web.WebPages;

namespace CodeArt.Web.WebPages.Xaml
{
    internal interface IPageProxy
    {
        /// <summary>
        /// 处理页面请求
        /// </summary>
        void Process(WebPageContext context);
    }
}
