using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;

namespace CodeArt.Web.WebPages
{
    public static class WebPageExtensions
    {
        private static Dictionary<string,bool> _extensions = new Dictionary<string,bool>();

        /// <summary>
        /// 注册一个webPage的扩展名
        /// </summary>
        /// <param name="extension"></param>
        public static void Register(string extension)
        {
            if (!_extensions.ContainsKey(extension))
            {
                lock (_extensions)
                {
                    if (!_extensions.ContainsKey(extension))
                    {
                        if (extension.StartsWith(".")) extension = extension.Substring(1);
                        _extensions.Add(extension, true);
                        _extensions.Add(string.Format(".{0}", extension), true);
                    }
                }
            }
        }

        public static bool IsValid(string extension)
        {
            bool result = _extensions.ContainsKey(extension);
            if (result) return result;
            //由于是静态方法，为了保证线程安全，所以多一次加锁的读取
            lock (_extensions)
            {
                return _extensions.ContainsKey(extension);
            }
        }

    }
}
