using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 考虑到性能问题，没有读取锁
    /// </summary>
    public static class WebPageRouterFactory
    {
        /// <summary>
        /// 页面路由器，请保证<paramref name="router"/>是单例的
        /// </summary>
        /// <param name="router"></param>
        public static void RegisterRouter(IWebPageRouter router)
        {
            SafeAccessAttribute.CheckUp(router.GetType());
            _instance = router;
        }

        public static IWebPageRouter CreateRouter()
        {
            if (_instance == null) return VirtualPathRouter.Instance;
            return _instance;
        }

        private static IWebPageRouter _instance = null;

    }
}
