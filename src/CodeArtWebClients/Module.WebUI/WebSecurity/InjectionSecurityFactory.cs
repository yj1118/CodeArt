using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;

using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    /// <summary>
    /// 注入式的压缩工厂
    /// </summary>
    internal class InjectionSecurityFactory : IWebSecurityFactory
    {
        private InjectionSecurityFactory() { }

        public static InjectionSecurityFactory Instance = new InjectionSecurityFactory();

        private IWebSecurity _security = null;


        /// <summary>
        /// 安全验证器，请保证security是单例的
        /// </summary>
        /// <param name="security"></param>
        public void Register(IWebSecurity security)
        {
            _security = security;
        }

        public IWebSecurity Create(WebPageContext context)
        {
            return _security ?? WebSecurityPrincipal.Instance;
        }
    }
}
