using System;
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Net;

using CodeArt.IO;

namespace CodeArt.Web
{
    public static class WebUtil
    {
        public static HttpContext CreateHttpContext(string url, TextWriter writer)
        {
            string queryString = string.Empty;
            int queryPosition = url.IndexOf('?');
            if (queryPosition > -1) queryString = url.Substring(queryPosition + 1);

            HttpRequest request = new HttpRequest(string.Empty, url, queryString);
            HttpResponse response = new HttpResponse(writer);
            return new HttpContext(request, response);
        }

        public static HttpContext CreateHttpContext(string url)
        {
            return CreateHttpContext(url, new StringWriter());
        }

        /// <summary>
        /// 正确的得到web查询参数
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static NameValueCollection ProcessQueryString(NameValueCollection rawQuery)
        {
            NameValueCollection query = new NameValueCollection(rawQuery.Count);
            foreach (var key in rawQuery.AllKeys)
            {
                var rawValue = rawQuery[key];
                string value = System.Web.HttpUtility.UrlDecode(rawValue, Encoding.UTF8);
                query.Add(key, value);
            }
            return query;

            //以下代码在?message=调用的目标发生了异常。&lt;br/&gt;尚未登录 的时候有BUG
            //if (queryString.Length == 0) return new NameValueCollection();
            //int questionPos = queryString.IndexOf('?');
            //if (questionPos == -1) return new NameValueCollection();
            //string[] paras = queryString.Substring(questionPos + 1).Split('&');
            //NameValueCollection query = new NameValueCollection(paras.Length);
            //foreach (string para in paras)
            //{
            //    int pos = para.IndexOf('=');
            //    string name = para.Substring(0, pos);
            //    string value = System.Web.HttpUtility.UrlDecode(para.Substring(pos + 1), Encoding.UTF8);
            //    query.Add(name, value);
            //}
            //return query;
        }

        public static string GetQueryString(NameValueCollection query)
        {
            if (query.Count == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < query.Count; i++)
            {
                sb.AppendFormat("{0}={1}&", query.Keys[i], HttpUtility.UrlEncode(query[i], Encoding.UTF8));
            }
            sb.Length--;
            return sb.ToString();
        }

        /// <summary>
        /// 转为查询字符串的格式
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection collection)
        {
            return GetQueryString(collection);
        }

        public static string ToQueryString(this NameValueCollection collection, string path)
        {
            var queryString = GetQueryString(collection);
            return AddQuery(path, queryString);
        }

        public static string Combine(params string[] paths)
        {
            if (paths.Length == 0) return string.Empty;
            Uri uri = new Uri(paths[0]);
            for (int i = 1; i < paths.Length; i++)
                uri = new Uri(uri, paths[i]);
            return uri.OriginalString;
        }

        public static string AddQuery(string path,string queryString)
        {
            if (string.IsNullOrEmpty(queryString)) return path;
            if (path.IndexOf('?') == -1) return string.Format("{0}?{1}", path, queryString);
            return string.Format("{0}&{1}", path, queryString);
        }

        public static string GetPathExtension(string path)
        {
            return IOUtil.GetExtension(path);
        }

        #region cookie

        /// <summary>
        /// 默认的cookie机制有缺陷，使用Get方法会自动生成一个无效的cookie；
        /// 使用该扩展方法可以避免此问题,并且还能获取一个有效的cookie
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static HttpCookie GetValid(this HttpCookieCollection cookies, string name)
        {
            //不能直接使用response.Cookies.Get(name),因为该方法会自动创建name的默认cookie
            for(var i=0;i<cookies.Count;i++)
            {
                var cookie = cookies[i];
                if (cookie.Name == name && IsValidCookie(cookie))
                    return cookie;
            }
            return null;
        }

        private static bool IsValidCookie(HttpCookie cookie)
        {
            //cookie存在并且没有过期(时间最小值代表不过期)
            return !string.IsNullOrEmpty(cookie.Value) && (cookie.Expires == DateTime.MinValue || cookie.Expires > DateTime.Now);
        }

        #endregion

        /// <summary>
        /// 获取当前站点的物理路径
        /// </summary>
        /// <returns></returns>
        public static string GetAbsolutePath()
        {
            //return HttpRuntime.AppDomainAppPath; 在新建的appDomain里不能使用
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string MapVirtualPath(string absolutePath)
        {
            return absolutePath.Substring(AppDomain.CurrentDomain.BaseDirectory.Length - 1).Replace("\\", "/");
        }

        public static string MapAbsolutePath(string virtualPath)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,virtualPath.TrimStart('/').Replace("/","\\"));
        }

        /// <summary>
        /// 发送一个POST请求
        /// </summary>
        /// <returns></returns>
        public static string SendPost(string url, string postString, Encoding encoding)
        {
            byte[] responseData = null;
            var postData = encoding.GetBytes(postString);
            using (WebClient web = new WebClient())
            {
                web.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可
                try
                {
                    responseData = web.UploadData(url, "POST", postData);//得到返回字符流
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return encoding.GetString(responseData);
        }

        /// <summary>
        /// 发送一个GET请求
        /// </summary>
        /// <returns></returns>
        public static byte[] SendGet(string url)
        {
            byte[] responseData = null;
            using (WebClient web = new WebClient())
            {
                try
                {
                    responseData = web.DownloadData(url);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return responseData;
        }


        /// <summary>
        /// 以后升级可以设置编码、超时时间的设置
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] SendPost(string address, byte[] data)
        {
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.ContentLength = data.Length;
            request.Timeout = 30000; //30秒超时

            //发送POST数据  
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            byte[] responseData = null;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                using (Stream stream = response.GetResponseStream())
                {
                    responseData = stream.GetBytes(100);
                }
            }
            return responseData;
        }

    }
}
