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

namespace Module.WebUI.Xaml
{
    public class WebFileInitializer : WebFileInitializerBase
    {
        protected override bool IsDownload(WebPageContext context)
        {
            return true;
        }

        protected override string GetFileName(WebPageContext context)
        {
            return context.Request.QueryString["name"] ?? context.Request.QueryString["key"];
        }

        protected override string GetETag(WebPageContext context)
        {
            return context.Request.QueryString["key"].ToBase64(Encoding.UTF8);
        }


        protected override string GetExtension(WebPageContext context)
        {
            string key = context.Request.QueryString["key"];
            if (key == null) return base.GetExtension(context);
            return PathUtil.GetExtension(key);
        }

    }
}
