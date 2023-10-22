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
using CodeArt.Util;

namespace CodeArt.Web.WebPages
{
    public abstract class WebFileInitializerBase : WebPageInitializer
    {
        protected override void SetContentType(WebPageContext context)
        {
            context.Response.ContentType = "application/octet-stream";
        }


        public override void Init()
        {
            base.Init();
            var context = WebPageContext.Current;
            context.Response.AddHeader("Accept-Ranges", "bytes");
            context.Response.AddHeader("ETag", GetETag(context));
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            if(this.IsDownload(context))
            {
                string fileName = GetFileName(context);
                context.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", HttpUtility.UrlEncode(fileName, Encoding.UTF8)));
            }
        }

        /// <summary>
        /// 文件的唯一标识
        /// </summary>
        /// <returns></returns>
        protected abstract string GetETag(WebPageContext context);

        protected abstract bool IsDownload(WebPageContext context);

        protected abstract string GetFileName(WebPageContext context);

    }
}
