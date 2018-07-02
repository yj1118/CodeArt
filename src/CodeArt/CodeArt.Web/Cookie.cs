using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Web
{
    public static class Cookie
    {
        #region 设置项

        public static void SetItem(string name, string value, int minutes)
        {
            if (HttpContext.Current == null) return;
            HttpCookieCollection cookies = HttpContext.Current.Response.Cookies;
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;
            cookie.Domain = GetDomain();
            cookie.Secure = false;
            if (minutes > 0) cookie.Expires = DateTime.Now.AddMinutes(minutes);

            cookies.Add(cookie);//追加或者修改,其实不能直接修改一个Cookie，而是创建一个同名的 Cookie，并把该 Cookie 发送到浏览器，覆盖客户机上旧的 Cookie。 
        }

        public static void SetItem(string name, string value)
        {
            SetItem(name, value, 0);
        }

        #endregion 

        /// <summary>
        /// 获取项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetItem(string name)
        {
            if (HttpContext.Current == null) return null;
            string value = null;
            if (name != null)
            {
                HttpResponse response = HttpContext.Current.Response; //当前线程已设置
                HttpCookie cookie = response.Cookies.GetValid(name);
                if (cookie != null) value = cookie.Value;
                else
                {
                    HttpRequest request = HttpContext.Current.Request;
                    cookie = request.Cookies.GetValid(name);
                    if (cookie != null) value = cookie.Value;
                    else
                    {
                        //检查queryString,以便不能写入cookie的客户端访问cookie
                        string ck = request.QueryString["cookie"];
                        if (ck != null)
                        {
                            string[] temps = ck.Split(';');
                            foreach (string temp in temps)
                            {
                                string[] t = temp.Trim().Split('=');
                                if (t[0] == name)
                                {
                                    value = t[1];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// 只能设置过期，不能在服务器端删除，否则客户端回发后又有了
        /// 删除的cookie，在设置cookie的时候指定的Domain属性，一定要跟删除的时候一样
        /// </summary>
        /// <param name="name"></param>
        /// <param name="domain"></param>
        public static void RemoveItem(string name)
        {
            if (HttpContext.Current == null) return;
            HttpContext context = HttpContext.Current;
            HttpCookieCollection requestCookies = context.Request.Cookies;
            HttpCookieCollection responseCookies = context.Response.Cookies;

            HttpCookie requestCookie = context.Request.Cookies.Get(name);
            HttpCookie responseCookie = context.Response.Cookies.Get(name);

            if (requestCookie != null)
            {
                requestCookie.Domain = GetDomain();
                requestCookie.Expires = DateTime.Now.AddYears(-1);
            }

            if (responseCookie != null)
            {
                responseCookie.Domain = GetDomain();
                responseCookie.Expires = DateTime.Now.AddYears(-1);
            }
        }

        /// <summary>
        /// 移除所有cookie项
        /// </summary>
        public static void Clear()
        {
            if (HttpContext.Current == null) return;
            HttpCookieCollection requestCookies = HttpContext.Current.Request.Cookies;
            for (int i = 0; i < requestCookies.Count; i++)
            {
                string name = requestCookies[i].Name;
                RemoveItem(name);
            }
        }

        private static string GetDomain()
        {
            WebHost host = new WebHost(HttpContext.Current.Request.Url.Host);
            return string.Format(".{0}", host.Domain);
        }
    }
}
