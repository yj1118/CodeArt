using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    public interface IWebGuard
    {
        void Validate(WebPageContext context);
    }
}
