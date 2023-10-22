using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace CodeArt.Web.WebPages
{
    public class DelayClientCache : ClientCacheBase
    {
        private DelayClientCache() { }

        public static DelayClientCache Instance = new DelayClientCache();

        private int GetCacheMinutes(WebPageContext context)
        {
            return context.GetConfigValue<int>("Page", "delay", 0);
        }

        public override bool IsExpired(ResolveRequestCache controller)
        {
            string modified = null;
            if (TryGetModified(controller.Context.Request, out modified)
                    && IsCaching(modified, GetCacheMinutes(controller.Context)))
            {
                SetClientStatusCode(controller.Context.Response);
                return false;
            }
            return true;
        }

        private bool IsCaching(string modified,int cacheMinutes)
        {
            DateTime cacheTime = DateTime.Now;
            if (!DateTime.TryParse(modified, out cacheTime)) return false;//不是日期格式
            return cacheTime.AddMinutes(cacheMinutes) > DateTime.Now;
        }

        public override void SetCache(ResolveRequestCache controller)
        {
            SetClientCache(controller.Context.Response, GetCacheMinutes(controller.Context));
        }
    }
}