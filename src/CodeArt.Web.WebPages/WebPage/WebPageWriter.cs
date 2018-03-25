using System;
using System.Web;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;

using CodeArt.Util;
using CodeArt.IO;

namespace CodeArt.Web.WebPages
{
    internal sealed class WebPageWriter : IWebPageWriter
    {
        private WebPageWriter() { }

        public void Write(WebPageContext context, Stream content)
        {
            WriteContent(context, content);
            UpdateCache(context, content);
        }

        private void WriteContent(WebPageContext context, Stream content)
        {
            Stream output = context.Response.OutputStream;
            int bufferSize = 64000;
            byte[] buffer = new byte[bufferSize];
            int retval = content.ReadPro(buffer, 0, bufferSize);
            while (retval == bufferSize)
            {
                output.Write(buffer, 0, bufferSize);
                output.Flush();
                retval = content.ReadPro(buffer, 0, bufferSize);
            }
            // 写入剩余的字节。
            output.Write(buffer, 0, retval);
            output.Flush();
        }

        /// <summary>
        /// 更新缓存，由于Response.OutputStream无法读取，所以只有在此处更新缓存
        /// </summary>
        /// <param name="context"></param>
        /// <param name="content"></param>
        private void UpdateCache(WebPageContext context, Stream content)
        {
            if (context.AnErrorOccurred || !context.IsGET) return;
            content.Position = 0;
            var cache = context.Cache;
            cache.SetCache(content);
        }

        public static readonly IWebPageWriter Instance = new WebPageWriter();

    }
}
