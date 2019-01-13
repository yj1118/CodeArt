using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;

using System.Web;

using CodeArt.AOP;

namespace CodeArt.Web.WebPages
{
    public static class WebPageRouter
    {
        public static IHttpHandler GetHandler(WebPageContext context)
        {
            var route = WebPageRouterFactory.CreateRouter();
            return route.CreateHandler(context);
        }

        public static IAspect[] GetAspects(WebPageContext context)
        {
            var route = WebPageRouterFactory.CreateRouter();
            return route.CreateAspects(context);
        }

    }
}
