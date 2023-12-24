using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;
using System.Web.SessionState;

using System.Web;
using System.IO;

using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages
{
    internal class WebPageError : WebPage
    {
        private Exception _error;
        private int _statusCode = 0;

        public WebPageError(Exception error, int statusCode)
        {
            _error = error;
            _statusCode = statusCode;
        }

        public WebPageError(Exception error)
            : this(error, 500)
        {
        }


        protected override void ProcessError(Exception ex)
        {
            //如果错误页本身发生错误，那么直接抛出异常
            throw ex;
        }

        protected override void ProcessGET()
        {
            ProcessGET(WebPageContext.Current, _error, _statusCode);
        }

        protected override void ProcessPOST()
        {
            ProcessPOST(WebPageContext.Current, _error, _statusCode);
        }

        #region 静态方法

        public static void Process(Exception ex,int statusCode = 500)
        {
            var current = WebPageContext.Current;
            if (current.IsGET)
            {
                ProcessGET(current, ex, statusCode);
            }
            else
            {
                ProcessPOST(current, ex, statusCode);
            }
        }

        private static void ProcessGET(WebPageContext context, Exception ex,int statusCode)
        {
            context.Response.StatusCode = statusCode;
            var msg = ex.GetCompleteInfo();
            if (context.IsErrorPage)
            {
                //如果错误页本身发生错误
                context.Response.Write(msg);
            }
            else
            {
#if (DEBUG)
                context.Response.Write(msg);
#endif

#if (!DEBUG)
                context.Redirect(string.Format("/error.htm?error={0}", HttpUtility.UrlEncode(msg)));
#endif
            }
        }

        private static void ProcessPOST(WebPageContext context, Exception ex,int statusCode)
        {
            context.Response.StatusCode = statusCode;
            context.Response.Headers["statusCode"] = statusCode.ToString();

            var message = ex.GetCompleteInfo();
            context.Response.Write(message);
        }

        #endregion

    }

}
