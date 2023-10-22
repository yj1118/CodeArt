using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.Web.RPC
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 尝试从缓冲区中获取数据
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(string uniqueKey, out string value);

        /// <summary>
        /// 增加或修改缓冲区中的对象
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        bool AddOrUpdate(string uniqueKey, string value);


        /// <summary>
        /// 从缓冲区中移除对象并返回被移除的缓冲条目
        /// </summary>
        /// <param name="uniqueKey"></param>
        string Remove(string uniqueKey);

        /// <summary>
        /// 清空缓冲区
        /// </summary>
        void Clear();

    }
}
