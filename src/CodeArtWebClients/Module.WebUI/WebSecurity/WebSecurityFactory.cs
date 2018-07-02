using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;

using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    public static class WebSecurityFactory
    {
        public static IWebSecurity Create(WebPageContext context)
        {
            return InjectionSecurityFactory.Instance.Create(context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="security">请保证为单例的</param>
        public static void Register(IWebSecurity security)
        {
            InjectionSecurityFactory.Instance.Register(security);
        }

    }
}
