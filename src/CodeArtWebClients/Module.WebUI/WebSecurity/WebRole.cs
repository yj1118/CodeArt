using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    /// <summary>
    /// 基于角色的验证
    /// </summary>
    public sealed class WebRole : WebSecurityAspect
    {
        private string[] _roles = null;

        public WebRole(string[] roles)
        {
            _roles = roles;
        }


        public override void Validate(WebPageContext context)
        {
            var security = CreateSecurity(context);
            if (!security.InRole(context, _roles))
            {
                var redirect = ConfigurationManager.AppSettings["limited-redirect"];
                if (!string.IsNullOrEmpty(redirect))
                {
                    WebPageContext.Current.Redirect(redirect);
                    return;
                }
                throw new WebSecurityException("权限不足");
            }
        }
    }
}