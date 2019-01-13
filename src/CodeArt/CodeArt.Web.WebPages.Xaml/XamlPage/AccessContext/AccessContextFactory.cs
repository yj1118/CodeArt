using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 考虑到性能问题，没有读取锁
    /// </summary>
    public static class AccessContextFactory
    {
        public static void Register(IAccessContext context)
        {
            SafeAccessAttribute.CheckUp(context.GetType());
            _instance = context;
        }

        public static IAccessContext GetContext()
        {
            if (_instance == null) return WebContext.Instance;
            return _instance;
        }

        private static IAccessContext _instance = null;

    }
}
