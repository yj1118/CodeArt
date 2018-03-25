using System;
using System.Web;
using System.Text;


namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 
    /// </summary>
    public interface IServerCacheFactory
    {
        IServerCache Create(WebPageContext context);

        /// <summary>
        /// 获取缓存存储器
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        ICacheStorage CreateStorage(WebPageContext context);
    }
}
