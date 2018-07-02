using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;

using CodeArt.Concurrent;


namespace CodeArt.DomainDriven
{
    [SafeAccess]
    public class MemoryBuffer : IDomainBuffer
    {
        private MemoryCache _buffer;

        public MemoryBuffer()
        {
            _buffer = new MemoryCache("MemoryBuffer");
        }

        /// <summary>
        /// 尝试从缓冲区中获取数据
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGet(string uniqueKey, out BufferEntry value)
        {
            value = _buffer[uniqueKey] as BufferEntry;
            return value != null;
        }

        /// <summary>
        /// 追加或修改缓冲对象
        /// </summary>
        /// <param name="uniqueKey"></param>
        /// <param name="value"></param>
        /// <returns>返回执行完修改操作后，缓冲区中对应的数据</returns>
        public bool AddOrUpdate(string uniqueKey, BufferEntry value)
        {
            CacheItemPolicy policy = GetPolicy();
            _buffer.Set(uniqueKey, value, policy);
            return true;
        }

        private CacheItemPolicy GetPolicy()
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = TimeSpan.FromMinutes(10);
            return policy;
        }

        public BufferEntry Remove(string uniqueKey)
        {
            return _buffer.Remove(uniqueKey) as BufferEntry;
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
