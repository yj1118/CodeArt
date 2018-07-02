using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    /// <summary>
    /// 基于权限的验证
    /// </summary>
    public sealed class WebPermission : WebSecurityAspect
    {
        private string[] _permissions = null;

        public WebPermission(string[] permissions)
        {
            _permissions = permissions;
        }

        public override void Validate(WebPageContext context)
        {
            var security = CreateSecurity(context);
            if (!security.InPermission(context, _permissions)) throw new WebSecurityException("权限不足");
        }
    }
}