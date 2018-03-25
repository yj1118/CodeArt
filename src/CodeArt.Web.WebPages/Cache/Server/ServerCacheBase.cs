using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;


namespace CodeArt.Web.WebPages
{
    public abstract class ServerCacheBase : IServerCache
    {
        protected virtual string GetUrl(WebPageContext context)
        {
            return context.Request.Url.AbsoluteUri;
        }

        public abstract bool IsExpired(WebPageContext context, ICacheStorage storage);

        /// <summary>
        /// 读取缓存区中的流信息
        /// </summary>
        /// <returns></returns>
        public abstract Stream Read(WebPageContext context, ICacheStorage storage);

        /// <summary>
        /// 向缓存区中写入信息
        /// </summary>
        /// <param name="content"></param>
        public abstract void Write(WebPageContext context, Stream content, ICacheStorage storage);

    }
}