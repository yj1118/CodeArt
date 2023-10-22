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
    public class WebImageInitializer : WebFileInitializerBase
    {

        protected override string GetETag(WebPageContext context)
        {
            string key = context.Request.QueryString["key"];
            var width = context.GetQueryValue<int>("w", 0);
            var height = context.GetQueryValue<int>("h", 0);
            var cutType = context.GetQueryValue<int>("c", 1);
            return string.Format("{0}|{1}|{2}|{3}", key, width, height, cutType).ToBase64(Encoding.UTF8);
        }

        protected override string GetExtension(WebPageContext context)
        {
            string key = context.Request.QueryString["key"];
            if (key == null) return base.GetExtension(context);
            return PathUtil.GetExtension(key);
        }

        protected override bool IsDownload(WebPageContext context)
        {
            return false;
        }

        protected override string GetFileName(WebPageContext context)
        {
            throw new NotImplementedException();
        }

    }
}
