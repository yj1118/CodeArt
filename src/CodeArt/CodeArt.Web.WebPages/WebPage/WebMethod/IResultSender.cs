using System;
using System.Web;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Specialized;
using System.Reflection;


namespace CodeArt.Web.WebPages
{
    public interface IResultSender
    {
        void Send(WebPageContext context, object result);

        /// <summary>
        /// 发送错误信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        void SendError(WebPageContext context, string error);
    }
}
