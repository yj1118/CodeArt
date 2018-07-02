using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    public sealed class WebSecurityEmpty : IWebSecurity
    {
        private WebSecurityEmpty() { }

        /// <summary>
        /// 拥有角色
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public bool InRole(WebPageContext context, string[] roles)
        {
            return true;
        }

        public bool InPermission(WebPageContext context, string[] permissions)
        {
            return true;
        }


        public static WebSecurityEmpty Instance = new WebSecurityEmpty();

    }
}
