using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

using CodeArt.Concurrent;


namespace CodeArt.DomainDriven.DataAccess
{
    [SafeAccess]
    public class MemoryCacheProxy : ICache
    {
        private static MemoryCache _cache;

        static MemoryCacheProxy()
        {
            _cache = new MemoryCache("DataAccess");
        }


        public MemoryCacheProxy()
        {

        }

        /// <summary>
        /// 尝试从缓存区中获取数据
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(CachePolicy tip, string cacheKey, out CacheEntry value)
        {
            value = tip.NoCache ? null : (CacheEntry)_cache[cacheKey];
            return value != null;
        }

        /// <summary>
        /// 追加或修改缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="confirm"></param>
        /// <returns>返回执行完修改操作后，缓存区中对应的数据</returns>
        public bool AddOrUpdate(CachePolicy tip, string cacheKey, CacheEntry value)
        {
            if (tip.NoCache) return false;

            CacheItemPolicy policy = GetPolicy(tip);
            _cache.Set(cacheKey, value, policy);
            return true;
        }

        private CacheItemPolicy GetPolicy(CachePolicy tip)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            if (tip.NotRemovable)
            {
                policy.Priority = CacheItemPriority.NotRemovable;
            }
            else
            {
                policy.SlidingExpiration = tip.SlidingExpiration;
            }
            return policy;
        }             



        public CacheEntry Remove(CachePolicy tip, string cacheKey)
        {
           return (CacheEntry)_cache.Remove(cacheKey);
        }

    }
}
