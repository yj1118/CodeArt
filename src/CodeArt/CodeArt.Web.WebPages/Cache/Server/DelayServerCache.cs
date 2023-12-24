using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;


namespace CodeArt.Web.WebPages
{
    public class DelayServerCache : ServerCacheBase
    {
        private DelayServerCache() { }

        public static DelayServerCache Instance = new DelayServerCache();


        /// <summary>
        /// 检查缓存是否过期
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true:缓存已过期;false:缓存未过期</returns>
        public override bool IsExpired(ResolveRequestCache controller, ICacheStorage storage)
        {
            DateTime cacheTime = DateTime.Now;
            if (TryGetCacheTime(controller, out cacheTime, storage)
                    && IsCaching(cacheTime, GetCacheMinutes(controller)))
            {
                return false;
            }
            return true;
        }

        private int GetCacheMinutes(ResolveRequestCache controller)
        {
            return controller.Context.GetConfigValue<int>("Page", "delay", 0);
        }

        protected bool TryGetCacheTime(ResolveRequestCache controller, out DateTime cacheTime, ICacheStorage storage)
        {
            var variable = new CacheVariable(controller.PageKey, controller.CompressionType, controller.Device);
            return storage.TryGetLastModified(variable, out cacheTime);
        }

        private bool IsCaching(DateTime cacheTime, int cacheMinutes)
        {
            return cacheTime.AddMinutes(cacheMinutes) > DateTime.Now;
        }

        /// <summary>
        /// 读取缓存区中的流信息
        /// </summary>
        /// <returns></returns>
        public override Stream Read(ResolveRequestCache controller, ICacheStorage storage)
        {
            var variable = new CacheVariable(controller.PageKey, controller.CompressionType, controller.Device);
            return storage.Read(variable);
        }

        /// <summary>
        /// 向缓存区中写入信息
        /// </summary>
        /// <param name="content"></param>
        public override void Write(ResolveRequestCache controller, Stream content, ICacheStorage storage)
        {
            var variable = new CacheVariable(controller.PageKey, controller.CompressionType, controller.Device);
            storage.Update(variable, content);
        }
    }
}