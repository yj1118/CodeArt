using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web.Hosting;
using System.IO;
using System.Security.AccessControl;

using System.Web;

using CodeArt.IO;

[assembly: PreApplicationStartMethod(typeof(CodeArt.Web.WebPages.Static.PreApplicationStartCode), "Start")]

namespace CodeArt.Web.WebPages.Static
{
    public class PreApplicationStartCode
    {
        private static bool _startWasCalled;

        public static void Start()
        {
            if (!_startWasCalled)
            {
                _startWasCalled = true;
                RegisterCSS();
                RegisterJS();
            }
        }

        /// <summary>
        /// 为了防止内存回收引起的watcher对象丢失，所以需要追加到静态引用集合中
        /// </summary>
        private static List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        private static void RegisterCSS()
        {
            BuildProvider.RegisterBuildProvider(".css", typeof(StaticBuildProvider));
            WebPageLocatorFactory.RegisterLocator(".css", StaticPageLocator.Instance);
            WebPageExtensions.Register(".css");

#if(DEBUG)
            _watchers.Add(PageUtil.WatchFiles(".css", (file, virtaulPath) =>
            {
                if (!StaticBuildProvider.IsIgnore(virtaulPath))
                    StaticBuildProvider.RemoveCache();
            }));
#endif
        }

        private static void RegisterJS()
        {
            BuildProvider.RegisterBuildProvider(".js", typeof(StaticBuildProvider));
            WebPageLocatorFactory.RegisterLocator(".js", StaticPageLocator.Instance);
            WebPageExtensions.Register(".js");

#if(DEBUG)
            _watchers.Add(PageUtil.WatchFiles(".js", (file, virtaulPath) =>
            {
                if (!StaticBuildProvider.IsIgnore(virtaulPath))
                    StaticBuildProvider.RemoveCache();
            }));
#endif
        }

   
    }
}
