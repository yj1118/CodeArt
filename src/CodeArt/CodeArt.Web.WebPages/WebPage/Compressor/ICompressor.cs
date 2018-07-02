using System;
using System.IO;
using System.Web;

namespace CodeArt.Web.WebPages
{
    public interface ICompressor
    {
        /// <summary>
        /// 判断浏览器是否支持压缩模式
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool IsAccepted(WebPageContext context);

        /// <summary>
        /// 设置响应的头，以便浏览器识别该压缩模式
        /// </summary>
        /// <param name="response"></param>
        void SetEncoding(WebPageContext context);

        /// <summary>
        /// 压缩流
        /// </summary>
        /// <param name="source">原始流</param>
        void Compress(WebPageContext context, Stream source, Stream target);

    }
}
