using System;
using System.Web;
using System.Text;

using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWebSecurityFactory
    {
        IWebSecurity Create(WebPageContext context);
    }
}
