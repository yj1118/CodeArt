using System;
using System.Collections.Generic;
using System.Linq;

using CodeArt.Concurrent;

namespace CodeArt.ModuleNest
{
    /// <summary>
    /// 考虑到性能问题，没有读取锁
    /// </summary>
    public static class ModuleHandlerRouterFactory
    {
        /// <summary>
        /// 页面路由器，请保证<paramref name="router"/>是单例的
        /// </summary>
        /// <param name="router"></param>
        public static void RegisterRoute(IModuleHandlerRouter router)
        {
            SafeAccessAttribute.CheckUp(router.GetType());
            _instance = router;
        }

        public static IModuleHandlerRouter CreateRoute()
        {
            if (_instance == null) return AttributeMHRouter.Instance;
            return _instance;
        }

        private static IModuleHandlerRouter _instance = null;

    }
}
