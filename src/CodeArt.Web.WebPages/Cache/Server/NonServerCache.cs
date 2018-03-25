using System;
using System.Web;
using System.IO;


namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NonServerCache : IServerCache
    {
        private NonServerCache() { }

        public bool IsExpired(WebPageContext context, ICacheStorage storage)
        {
            return true;
        }

        public Stream Read(WebPageContext context, ICacheStorage storage)
        {
            return null;
        }

        public void Write(WebPageContext context, Stream content, ICacheStorage storage) { }

        public void Delete(WebPageContext context, ICacheStorage storage) { }

        public static readonly IServerCache Instance = new NonServerCache();

    }
}
