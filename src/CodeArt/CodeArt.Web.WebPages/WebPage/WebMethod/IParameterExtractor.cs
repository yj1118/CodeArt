using System;
using System.Web;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Specialized;
using System.Reflection;

using CodeArt.DTO;

namespace CodeArt.Web.WebPages
{
    public interface IParameterExtractor
    {
        /// <summary>
        /// 从请求的中分析提取参数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="prms"></param>
        /// <returns></returns>
        DTObject ParseArguments(WebPageContext context);
    }
}
