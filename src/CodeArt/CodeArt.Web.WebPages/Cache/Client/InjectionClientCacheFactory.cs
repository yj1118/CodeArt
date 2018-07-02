using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;


namespace CodeArt.Web.WebPages
{
    internal class InjectionClientCacheFactory : IClientCacheFactory
    {
        protected InjectionClientCacheFactory() { }

        public static InjectionClientCacheFactory Instance = new InjectionClientCacheFactory();


        private static Dictionary<string, IClientCache> _caches = new Dictionary<string, IClientCache>(5);

        /// <summary>
        /// 注册缓存处理器，请保证cache是单例的
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="cache"></param>
        public void RegisterCache(string extension, IClientCache cache)
        {
            if (!_caches.ContainsKey(extension))
            {
                lock (_caches)
                {
                    if (!_caches.ContainsKey(extension))
                    {
                        if (extension.StartsWith(".")) extension = extension.Substring(1);
                        _caches.Add(extension, cache);
                        _caches.Add(string.Format(".{0}", extension), cache);
                    }
                }
            }
        }

        public virtual IClientCache Create(WebPageContext context)
        {
            var value = context.GetConfigValue<string>("Page", "ClientCache", string.Empty).ToLower();
            if (!string.IsNullOrEmpty(value))
            {
                switch (value)
                {
                    case "forever": return ForeverClientCache.Instance;
                    case "delay": return DelayClientCache.Instance;
                    default: return NonClientCache.Instance;
                }
            }

            //通过后缀名获取
            IClientCache cache = null;
            if (_caches.TryGetValue(context.PathExtension, out cache))
                return cache;    

            return NonClientCache.Instance;
        }
    }
}
