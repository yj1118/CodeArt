using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.AOP;
using CodeArt.Web.WebPages;

namespace CodeArt.Web.WebPages.Static
{
    public class StaticPage : WebText
    {
        public StaticPage()
        {
        }

        protected override string GetTextContent(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }
            return StaticBuildProvider.GetDynamicCode(this.VirtualPath);
        }

        protected override object CallWebMethod(object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 默认的页面处理器，该对象是线程安全的
        /// </summary>
        public readonly static StaticPage Instance = new StaticPage();

    }
}
