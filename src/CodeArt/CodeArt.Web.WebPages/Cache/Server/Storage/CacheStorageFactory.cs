using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;


namespace CodeArt.Web.WebPages
{
    internal static class CacheStorageFactory
    {
        private static Dictionary<string, ICacheStorage> _storage = new Dictionary<string, ICacheStorage>(5);

        /// <summary>
        /// 注册缓存存储器名称，请保证storage是单例的,使用该机制可以通过名称得到缓存存储器的实现
        /// </summary>
        /// <param name="name"></param>
        /// <param name="storage"></param>
        public static void Register(string name, ICacheStorage storage)
        {
            if (!_storage.ContainsKey(name))
            {
                lock (_storage)
                {
                    if (!_storage.ContainsKey(name))
                    {
                        _storage.Add(name, storage);
                    }
                }
            }
        }

        public static ICacheStorage Create(string name)
        {
            ICacheStorage storage = null;
            if (_storage.TryGetValue(name, out storage))
                return storage;
            throw new WebCacheException("没有找到名称为 " + name + " 的缓存存储器");
        }

        /// <summary>
        /// 获取所有的缓存储存器
        /// </summary>
        /// <returns></returns>
        public static IList<ICacheStorage> Creates()
        {
            return _storage.Values.ToList();
        }

        static CacheStorageFactory()
        {
            //Register("memory", MemoryStorage.Instance);
        }

    }
}
