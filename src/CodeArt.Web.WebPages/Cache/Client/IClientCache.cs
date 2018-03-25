using System;
using System.Text;

using System.Web;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClientCache
    {
        /// <summary>
        /// 检查缓存是否过期
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true:缓存已过期;false:缓存未过期</returns>
        bool IsExpired(WebPageContext context);

        /// <summary>
        /// 设置缓存
        /// </summary>
        void SetCache(WebPageContext context);
    }
}
