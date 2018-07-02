using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;


namespace CodeArt.Web.WebPages
{
    internal class InjectionServerCacheFactory : IServerCacheFactory
    {
        protected InjectionServerCacheFactory() { }

        public static InjectionServerCacheFactory Instance = new InjectionServerCacheFactory();


        private static Dictionary<string, IServerCache> _caches = new Dictionary<string, IServerCache>(5);

        /// <summary>
        /// 注册缓存处理器，请保证cache是单例的
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="cache"></param>
        public void RegisterCache(string extension, IServerCache cache)
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

        public virtual IServerCache Create(WebPageContext context)
        {
            var value = context.GetConfigValue<string>("Page", "ServerCache", string.Empty).ToLower();
            if (!string.IsNullOrEmpty(value))
            {
                switch (value)
                {
                    case "forever": return ForeverServerCache.Instance;
                    case "delay": return DelayServerCache.Instance;
                    default: return NonServerCache.Instance;
                }
            }

            //通过后缀名获取
            IServerCache cache = null;
            if (_caches.TryGetValue(context.PathExtension, out cache))
                return cache;

            return NonServerCache.Instance;
        }


        #region 存储器

        private static Dictionary<string, ICacheStorage> _storages = new Dictionary<string, ICacheStorage>(5);

        /// <summary>
        /// 注册缓存存储器，请保证storage是单例的
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="storage"></param>
        public void RegisterStorage(string extension, ICacheStorage storage)
        {
            if (!_storages.ContainsKey(extension))
            {
                lock (_storages)
                {
                    if (!_storages.ContainsKey(extension))
                    {
                        if (extension.StartsWith(".")) extension = extension.Substring(1);
                        _storages.Add(extension, storage);
                        _storages.Add(string.Format(".{0}", extension), storage);
                    }
                }
            }
        }

        public virtual ICacheStorage CreateStorage(WebPageContext context)
        {
            var value = context.GetConfigValue<string>("Page", "CacheStorage", string.Empty).ToLower();
            if (!string.IsNullOrEmpty(value)) return CacheStorageFactory.Create(value); //通过存储器名称获取

            //通过后缀名获取
            ICacheStorage storage = null;
            if (_storages.TryGetValue(context.PathExtension, out storage))
                return storage;

            return StorageEmpty.Instance;
        }

        #endregion




    }
}
