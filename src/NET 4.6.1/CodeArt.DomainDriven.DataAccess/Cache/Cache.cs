using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    internal static class Cache
    {
        private static ICache _cache;

        static Cache()
        {
            _cache = DataAccessConfiguration.Current.GetCache() ?? new MemoryCacheProxy();
        }


        /// <summary>
        /// 从缓存区中创建或者获取数据
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="level"></param>
        /// <param name="load"></param>
        /// <returns></returns>
        private static object GetOrAdd(ObjectRepositoryAttribute tip, string cacheKey, int dataVersion, Func<object> load)
        {
            if (tip.NoCache) return load();

            CacheEntry result = null;
            if (_cache.TryGet(tip.CachePolicy, cacheKey, out result))
            {
                if (result.DataVersion >= dataVersion)
                    return result.Object;
            }

            //更新缓存
            var obj = load();
            result = new CacheEntry(obj, dataVersion);
            _cache.AddOrUpdate(tip.CachePolicy, cacheKey, result);
            return result.Object;
        }

        private static void Add(CachePolicy policy, string cacheKey, object obj)
        {
            var result = new CacheEntry(obj, 1); //主动追加的数据，版本号从1开始
            _cache.AddOrUpdate(policy, cacheKey, result);
        }


        private static string GetCacheKey(ObjectRepositoryAttribute tip, object id)
        {
            return string.Format("{0}+{1}", tip.ObjectType.FullName, id.ToString());
        }

        private static string GetCacheKey(ObjectRepositoryAttribute tip, object rootId, object id)
        {
            return string.Format("{0}+{1}+{2}", tip.ObjectType.FullName, rootId.ToString(), id.ToString());
        }

        public static object GetOrCreate(ObjectRepositoryAttribute tip, object id, int dataVersion, Func<object> load)
        {
            var cacheKey = GetCacheKey(tip, id);
            return GetOrAdd(tip, cacheKey, dataVersion, load);
        }

        public static object GetOrCreate(ObjectRepositoryAttribute tip, object rootId, object id, int dataVersion, Func<object> load)
        {
            var cacheKey = GetCacheKey(tip, rootId, id);
            return GetOrAdd(tip, cacheKey, dataVersion, load);
        }

        /// <summary>
        /// 将对象加入缓存
        /// </summary>
        /// <param name="obj"></param>
        public static void Add(ObjectRepositoryAttribute tip, object rootId, object id, IDomainObject obj)
        {
            var cacheKey = GetCacheKey(tip, rootId, id);
            Add(tip.CachePolicy, cacheKey, obj);
        }

        /// <summary>
        /// 将对象加入缓存
        /// </summary>
        /// <param name="obj"></param>
        public static void Add(ObjectRepositoryAttribute tip, object id, IDomainObject obj)
        {
            var cacheKey = GetCacheKey(tip, id);
            Add(tip.CachePolicy, cacheKey, obj);
        }


        public static void Remove(ObjectRepositoryAttribute tip, object id)
        {
            var cacheKey = GetCacheKey(tip, id);
            _cache.Remove(tip.CachePolicy, cacheKey);
        }

        public static void Remove(ObjectRepositoryAttribute tip, object rootId, object id)
        {
            var cacheKey = GetCacheKey(tip, rootId, id);
            _cache.Remove(tip.CachePolicy, cacheKey);
        }
    }
}
