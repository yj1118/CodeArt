using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Log;

using CodeArt.AOP;
using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    /// <summary>
    /// cookie模拟器
    /// </summary>
    internal static class CookieSimulator
    {
        /// <summary>
        /// 获得代码形式的cookies
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetCode(HttpRequest request)
        {
            //获得环境上下文的代码
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.Append("[");
                var cookies = request.Cookies;
                foreach (HttpCookie cookie in cookies)
                {
                    FillCookieCode(sb, cookie);
                    sb.Append(",");
                }
                if (cookies.Count > 0) sb.Length--;
                sb.Append("]");
                return sb.ToString();
            }
        }

        private static void FillCookieCode(StringBuilder code, HttpCookie cookie)
        {
            code.Append("{");
            code.AppendFormat("name:'{0}',", cookie.Name);
            code.AppendFormat("value:'{0}',", cookie.Value);
            code.AppendFormat("domain:'{0}',", cookie.Domain);
            code.AppendFormat("expires:{0}", JSON.GetCode(cookie.Expires));
            code.Append("}");
        }



    }
}
