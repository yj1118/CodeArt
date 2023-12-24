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
    internal static class ContextConverter
    {
        public static string GetCode(ITenancy tenancy, WebPageContext ctx)
        {
            //获得环境上下文的代码
            var request = ctx.Request;
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.Append("{");
                FillTenancyCode(sb, tenancy);
                sb.Append(",");
                FillRequestCode(sb, ctx);
                sb.Append("}");
                return sb.ToString();
            }
        }

        private static void FillTenancyCode(StringBuilder sb, ITenancy tenancy)
        {
            sb.Append("tenancy:{");
            sb.AppendFormat("id:'{0}'", tenancy.Id);
            sb.Append("}");
        }

        private static void FillRequestCode(StringBuilder sb, WebPageContext ctx)
        {
            var request = ctx.Request;
            sb.Append("request:{");
            sb.AppendFormat("url:'{0}',", request.RawUrl);
            sb.AppendFormat("path:'{0}',", ctx.VirtualPath);
            sb.AppendFormat("type:'{0}',", request.RequestType);
            sb.AppendFormat("cookies:{0},", CookieSimulator.GetCode(request));
            sb.AppendFormat("mobile:{0},", JSON.GetCode(ctx.IsMobileDevice));
            sb.AppendFormat("extension:'{0}',", request.CurrentExecutionFilePathExtension);
            sb.Append("query:{");
            FillQueryStringCode(sb, request);
            sb.Append("}");
            sb.Append("}");
        }

        private static void FillQueryStringCode(StringBuilder sb,HttpRequest request)
        {
            var rawQuery = request.QueryString;
            foreach (var key in rawQuery.AllKeys)
            {
                var rawValue = rawQuery[key];
                string value = System.Web.HttpUtility.UrlDecode(rawValue, Encoding.UTF8);
                sb.AppendFormat("{0}:{1},", key, JSON.GetCode(value));
            }

            if(rawQuery.Count > 0)
            {
                sb.Length--;
            }
        }

    }
}
