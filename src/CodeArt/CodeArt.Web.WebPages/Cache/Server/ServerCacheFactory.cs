using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;


namespace CodeArt.Web.WebPages
{
    public static class ServerCacheFactory
    {
        public static IServerCache Create(WebPageContext context)
        {
            IServerCache serverCache = null;
            var config = WebPagesConfiguration.Global.PageConfig;
            if (config == null || config.ServerCacheFactory == null)
                serverCache = InjectionServerCacheFactory.Instance.Create(context);
            else
            {
                IServerCacheFactory factory = config.ServerCacheFactory.GetInstance<IServerCacheFactory>();
                serverCache = factory.Create(context);
                if (serverCache == null) serverCache = NonServerCache.Instance; //配置文件没有设置的就由系统自动设置
            }
            return serverCache;
        }

        /// <summary>
        /// 注册缓存处理器，请保证cache是单例的
        /// </summary>
        public static void RegisterCache(string extension, IServerCache cache)
        {
            InjectionServerCacheFactory.Instance.RegisterCache(extension, cache);
        }

        /// <summary>
        /// 注册访问后缀对应的存储器信息
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="storage"></param>
        public static void RegisterStorage(string extension, ICacheStorage storage)
        {
            InjectionServerCacheFactory.Instance.RegisterStorage(extension, storage);
        }

        /// <summary>
        /// 注册存储器名称
        /// </summary>
        /// <param name="name"></param>
        /// <param name="storage"></param>
        public static void RegisterStorageName(string name, ICacheStorage storage)
        {
            CacheStorageFactory.Register(name, storage);
        }

        public static ICacheStorage CreateStorage(string name)
        {
            return CacheStorageFactory.Create(name);
        }

        public static IList<ICacheStorage> CreateStorages()
        {
            return CacheStorageFactory.Creates();
        }

    }
}
