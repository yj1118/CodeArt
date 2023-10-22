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

        public bool IsExpired(ResolveRequestCache controller, ICacheStorage storage)
        {
            return true;
        }

        public Stream Read(ResolveRequestCache controller, ICacheStorage storage)
        {
            return null;
        }

        public void Write(ResolveRequestCache controller, Stream content, ICacheStorage storage) { }

        public void Delete(ResolveRequestCache controller, ICacheStorage storage) { }

        public static readonly IServerCache Instance = new NonServerCache();

    }
}
