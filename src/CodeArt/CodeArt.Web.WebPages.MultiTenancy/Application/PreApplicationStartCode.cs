using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Web.WebPages;

[assembly: PreApplicationStartMethod(typeof(CodeArt.Web.WebPages.MultiTenancy.PreApplicationStartCode), "Start")]

namespace CodeArt.Web.WebPages.MultiTenancy
{
    public class PreApplicationStartCode
    {
        private static bool _startWasCalled;

        public static void Start()
        {
            if (!_startWasCalled)
            {
                _startWasCalled = true;
                WebPageRouterFactory.RegisterRouter(TenancyPageRouter.Instance);
            }
        }
    }
}
