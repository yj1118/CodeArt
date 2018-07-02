using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web.Hosting;
using System.IO;
using System.Security.AccessControl;
using System.Web;

[assembly: PreApplicationStartMethod(typeof(CodeArt.Web.WebPages.Xaml.PreApplicationStartCode), "Start")]

namespace CodeArt.Web.WebPages.Xaml
{
    public class PreApplicationStartCode
    {
        private static bool _startWasCalled;

        public static void Start()
        {
            if (!_startWasCalled)
            {
                _startWasCalled = true;
                RegisterHTM();
            }
        }

        /// <summary>
        /// 保留wathcer的引用，避免被回收引起监控的中断
        /// </summary>
        private static List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();

        private static void RegisterHTM()
        {
            WebPageLocatorFactory.RegisterLocator(".htm", XamlPageLocator.Instance);
            WebPageExtensions.Register(".htm");

            BuildProvider.RegisterBuildProvider(".htm", typeof(XPCBuildProvider));
            //在VS里执行生成时，App_Code下的cs文件会自动编译，如果某个文件生成失败，那么整个生成操作（包括htm）会终止并且报错
            //这就导致修改后的htm始终无法生成新的cs文件，因此，我们使用XPCBuildIgnoreProvider来让VS不执行cs的生成操作
            BuildProvider.RegisterBuildProvider(".cs", typeof(XPCBuildIgnoreProvider)); 

#if (DEBUG)
            _watchers.Add(PageUtil.WatchFiles(".htm", (file, virtualPath) =>
            {
                HtmlWatcher.Instance.OnChange(virtualPath);
            }));

            _watchers.Add(PageUtil.WatchFiles(".cs", (file, virtualPath) =>
            {
                CSharpWatcher.Instance.OnChange(virtualPath);
            }));
#endif
        }

    }
}
