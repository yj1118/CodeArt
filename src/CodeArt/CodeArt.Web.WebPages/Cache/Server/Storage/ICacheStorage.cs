using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace CodeArt.Web.WebPages
{
    public interface ICacheStorage
    {
        /// <summary>
        /// 得到最后一次缓存的时间
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="lastModified"></param>
        /// <returns></returns>
        bool TryGetLastModified(CacheVariable variable, out DateTime lastModified);

        /// <summary>
        /// 读取缓存区内容
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        Stream Read(CacheVariable variable);

        /// <summary>
        /// 更新缓存区内容
        /// </summary>
        /// <param name="variable"></param>
        void Update(CacheVariable variable, Stream content);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="content"></param>
        void Delete(CacheVariable variable);

        /// <summary>
        /// 删除全部缓存
        /// </summary>
        void DeleteAll();
    }
}