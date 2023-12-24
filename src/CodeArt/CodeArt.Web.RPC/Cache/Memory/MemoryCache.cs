using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;


namespace CodeArt.Web.RPC
{
    [SafeAccess]
    public class MemoryCache : ICache
    {
        private System.Runtime.Caching.MemoryCache _buffer;
        private TimeSpan _slidingExpiration;
        private MemoryCache(int minutes)
        {
            _slidingExpiration = minutes > 0 ? TimeSpan.FromMinutes(minutes) : TimeSpan.FromDays(360);
            _buffer = new System.Runtime.Caching.MemoryCache("MemoryCache - " + Guid.NewGuid());
        }

        public static MemoryCache CreateDelay(int minutes)
        {
            return new MemoryCache(minutes);
        }

        public static MemoryCache CreateForver()
        {
            return new MemoryCache(-1);
        }

        /// <summary>
        /// 尝试从缓冲区中获取数据
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(string uniqueKey, out string value)
        {
            value = _buffer.Get(uniqueKey) as string;
            return value != null;
        }

        /// <summary>
        /// 追加或修改缓冲对象
        /// </summary>
        /// <param name="uniqueKey"></param>
        /// <param name="value"></param>
        /// <returns>返回执行完修改操作后，缓冲区中对应的数据</returns>
        public bool AddOrUpdate(string uniqueKey, string value)
        {
            CacheItemPolicy policy = GetPolicy();
            _buffer.Set(uniqueKey, value, policy);
            return true;
        }

        private CacheItemPolicy GetPolicy()
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = _slidingExpiration;
            return policy;
        }

        public string Remove(string uniqueKey)
        {
            return _buffer.Remove(uniqueKey) as string;
        }

        public void Clear()
        {
            foreach (var item in _buffer)
            {
                _buffer.Remove(item.Key);
            }
        }
    }
}
