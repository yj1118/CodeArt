using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Web.WebPages;
using CodeArt.AppSetting;
using CodeArt.Security;

[assembly: PreAppSessionStart(typeof(RPC.Common.PreAppSessionStartCode), "Start")]

namespace RPC.Common
{
    public class PreAppSessionStartCode
    {
        public static void Start()
        {
            AppSession.Language = WebPageContext.Current.Request.Headers["c-language"] ?? CodeArt.AppContext.LocalLanguage;
        }
    }
}
