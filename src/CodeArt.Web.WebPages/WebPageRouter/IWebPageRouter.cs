using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;

using System.Web;

using CodeArt.AOP;

namespace CodeArt.Web.WebPages
{
    public interface IWebPageRouter
    {
        IHttpHandler CreateHandler(WebPageContext context);

        IAspect[] CreateAspects(WebPageContext context);

    }
}
