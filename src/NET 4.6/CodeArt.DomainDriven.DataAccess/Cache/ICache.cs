using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 请保证缓存对象的方法是线程安全的
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 尝试从缓存区中获取数据
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(CachePolicy tip, string cacheKey, out CacheEntry value);

        /// <summary>
        /// 增加或修改缓存区中的数据
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        bool AddOrUpdate(CachePolicy tip, string cacheKey, CacheEntry value);


        /// <summary>
        /// 移除缓存，并返回被移除的缓存条目
        /// </summary>
        /// <param name="cacheKey"></param>
        CacheEntry Remove(CachePolicy tip, string cacheKey);

    }
}
