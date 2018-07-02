using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 代码预处理器
    /// </summary>
    public interface ICodePreprocessor
    {
        /// <summary>
        /// 读取页面代码
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        string ReadCode(string virtualPath);

        /// <summary>
        /// 读取页面设置的信息
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        WebPageSetting ReadSetting(string virtualPath);

        /// <summary>
        /// 该请求的路径是否是有效的
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        bool IsValidPath(string virtualPath);
    }
}
