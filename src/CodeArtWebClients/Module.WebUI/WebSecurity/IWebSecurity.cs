using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    public interface IWebSecurity
    {
        /// <summary>
        /// 拥有角色
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        bool InRole(WebPageContext context, string[] roles);

        /// <summary>
        /// 拥有权限
        /// </summary>
        /// <param name="context"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        bool InPermission(WebPageContext context, string[] permissions);
    }
}
