using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Util;
using CodeArt.Runtime;

using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    public class WordInitializer : WebFileInitializerBase
    {
        protected override void SetContentType(WebPageContext context)
        {
            context.Response.ContentType = "application/msword";
        }

        protected override bool IsDownload(WebPageContext context)
        {
            return true;
        }

        protected override string GetFileName(WebPageContext context)
        {
            return context.Request.QueryString["name"];
        }

        protected override string GetETag(WebPageContext context)
        {
            return context.Request.QueryString["id"].ToBase64(Encoding.UTF8);
        }
        
        protected override string GetExtension(WebPageContext context)
        {
            return ".doc";
        }
    }
}
