using System;
using System.Web;
using System.Text;
using System.IO;


namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServerCache
    {
        /// <summary>
        /// 检查缓存是否过期
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true:缓存已过期;false:缓存未过期</returns>
        bool IsExpired(ResolveRequestCache controller, ICacheStorage storage);

        /// <summary>
        /// 读取缓存区中的流信息
        /// </summary>
        /// <returns></returns>
        Stream Read(ResolveRequestCache controller, ICacheStorage storage);

        /// <summary>
        /// 向缓存区中写入信息
        /// </summary>
        /// <param name="content"></param>
        void Write(ResolveRequestCache controller, Stream content, ICacheStorage storage);
    }
}
