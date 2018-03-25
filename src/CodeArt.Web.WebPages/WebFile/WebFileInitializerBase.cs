using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Runtime;

namespace CodeArt.Web.WebPages
{
    public abstract class WebFileInitializerBase : WebPageInitializer
    {
        public override void Init()
        {
            base.Init();
            var context = WebPageContext.Current;
            context.Response.AddHeader("Accept-Ranges", "bytes");
            context.Response.AddHeader("ETag", GetETag(context));
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
        }

        /// <summary>
        /// 文件的唯一标识
        /// </summary>
        /// <returns></returns>
        protected abstract string GetETag(WebPageContext context);
    }
}
