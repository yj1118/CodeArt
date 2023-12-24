using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.Web.WebPages.Xaml.Markup
{
    internal interface IXamReader
    {
        /// <summary>
        /// 加载xaml文件,生成对应的组件对象
        /// </summary>
        /// <param name="xaml"></param>
        /// <returns></returns>
        object Read(string xaml);

        /// <summary>
        /// 根据xaml内容，加载obj信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xaml"></param>
        void Load(object obj, string xaml);

    }

}
