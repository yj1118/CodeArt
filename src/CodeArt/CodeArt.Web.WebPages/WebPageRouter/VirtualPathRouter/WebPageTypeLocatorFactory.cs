using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;

namespace CodeArt.Web.WebPages
{
    public static class WebPageLocatorFactory
    {
        /// <summary>
        /// 注册页面类型定位器，请保证<paramref name="locator"/>是单例的
        /// </summary>
        /// <param name="locator"></param>
        public static void RegisterLocator(string extension, IWebPageLocator locator)
        {
            if (!_locators.ContainsKey(extension))
            {
                lock (_locators)
                {
                    if (!_locators.ContainsKey(extension))
                    {
                        if (extension.StartsWith(".")) extension = extension.Substring(1);
                        _locators.Add(extension, locator);
                        _locators.Add(string.Format(".{0}", extension), locator);
                    }
                }
            }
        }

        public static IWebPageLocator CreateLocator(string extension)
        {
            IWebPageLocator locator = WebPagesConfiguration.Global.PageConfig.GetPageLocator(extension); //优先从配置中读取
            if (locator != null) return locator;
            

            if (_locators.TryGetValue(extension, out locator))
                return locator;

            //由于是静态方法，要保证线程安全，所以锁定，再读取
            lock (_locators)
            {
                if (_locators.TryGetValue(extension, out locator))
                    return locator;
            }
            return WebPageTypeLocatorEmpty.Instance;
        }

        private static Dictionary<string, IWebPageLocator> _locators = new Dictionary<string, IWebPageLocator>(5);

    }
}
