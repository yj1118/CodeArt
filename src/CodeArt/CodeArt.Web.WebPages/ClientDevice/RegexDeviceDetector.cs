using System;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 基于正则的验证
    /// </summary>
    public sealed class RegexDeviceDetector : IDeviceDetector
    {
        /// <summary>
        /// 反编译后，发现regex是线程安全的(待确定)
        /// </summary>
        //private static Regex _regex = new Regex(@"(iemobile|iphone|ipod|android|nokia|sonyericsson|blackberry|samsung|sec\-|windows ce|motorola|mot\-|up.b|midp\-)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Pool<Regex> _regexPool = new Pool<Regex>(() =>
        {
            return new Regex(@"(iemobile|iphone|ipod|android|nokia|sonyericsson|blackberry|samsung|sec\-|windows ce|motorola|mot\-|up.b|midp\-)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }, (reg, phase) =>
        {
            return true;
        }, new PoolConfig()
        {

        });

        private RegexDeviceDetector() { }

        public ClientDevice Detect(WebPageContext context)
        {
            ClientDevice mobile = new ClientDevice();
            var request = context.Request;
            if (request.Browser.IsMobileDevice) mobile.IsMobile = true;
            else
            {
                var userAgent = request.UserAgent;
                if (!string.IsNullOrEmpty(userAgent))
                {
                    using(var item = _regexPool.Borrow())
                    {
                        Regex reg = item.Item;
                        if (reg.IsMatch(userAgent)) mobile.IsMobile = true;
                    }
                }
            }
            return mobile;
        }

        public static IDeviceDetector Instance = new RegexDeviceDetector();
    }
}
