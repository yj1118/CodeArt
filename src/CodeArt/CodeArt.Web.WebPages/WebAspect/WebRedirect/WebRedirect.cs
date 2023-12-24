using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AOP;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    [SafeAccess()]
    public sealed class WebRedirect : IAspect
    {
        private string _url;

        public WebRedirect(string url)
        {
            _url = url;
        }


        public void Before()
        {
            throw new RedirectException(_url);
        }

        public void After() { }
    }
}