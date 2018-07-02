using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AOP;
using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    public abstract class WebSecurityAspect : IAspect
    {
        public void Before()
        {
            Validate(WebPageContext.Current);
        }

        public void After() { }

        protected IWebSecurity CreateSecurity(WebPageContext context)
        {
            return WebSecurityFactory.Create(context);
        }

        public abstract void Validate(WebPageContext context);
    }
}