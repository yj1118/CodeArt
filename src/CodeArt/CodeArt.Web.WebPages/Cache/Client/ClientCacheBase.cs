using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 该类是线程安全的
    /// </summary>
    public abstract class ClientCacheBase : IClientCache
    {
        public abstract bool IsExpired(ResolveRequestCache controller);

        public abstract void SetCache(ResolveRequestCache controller);

        protected bool TryGetModified(HttpRequest request,out string modified)
        {
            modified = request.Headers["If-Modified-Since"];
            if (modified != null && modified != "0") return true;
            return false;
        }
        
        protected void SetClientStatusCode(HttpResponse response)
        {
            response.StatusCode = 304;
        }

        protected void SetClientCache(HttpResponse response, int minutes)
        {
            if (minutes > 0)
            {
                response.Cache.SetLastModified(DateTime.Now);
                response.Cache.SetMaxAge(new TimeSpan(0, minutes, 0));

                //以指定响应能由客户端和共享（代理）缓存进行缓存。
                response.Cache.SetCacheability(HttpCacheability.Public);
                //可调过期策略
                response.Cache.SetSlidingExpiration(true);

            }
        }
    }
}