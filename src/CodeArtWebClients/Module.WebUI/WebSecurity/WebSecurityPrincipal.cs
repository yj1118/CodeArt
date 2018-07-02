using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AOP;
using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    /// <summary>
    /// 基于当前登陆者信息的验证，该验证的缺陷在于，如果改变了登陆者的权限信息，那么需要登陆者重新登录才能生效
    /// 优点在于比远程访问登陆者的权限信息比起来性能比较好
    /// </summary>
    public class WebSecurityPrincipal : IWebSecurity
    {
        private WebSecurityPrincipal() { }

        /// <summary>
        /// 拥有角色
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public bool InRole(WebPageContext context, string[] roles)
        {
            foreach(var role in roles)
            {
                if (Principal.InRole(role)) return true;
            }
            return false;
        }

        public bool InPermission(WebPageContext context, string[] permissions)
        {
            throw new ApplicationException("尚未实现WebSecurityPrincipal.InPermission");
        }


        public static WebSecurityPrincipal Instance = new WebSecurityPrincipal();
    }
}