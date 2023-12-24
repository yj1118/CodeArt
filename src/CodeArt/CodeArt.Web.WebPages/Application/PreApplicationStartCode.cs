using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;

using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System.IO;

[assembly: PreApplicationStartMethod(typeof(CodeArt.Web.WebPages.PreApplicationStartCode), "Start")]

namespace CodeArt.Web.WebPages
{
    public class PreApplicationStartCode
    {
        private static bool _startWasCalled;

        /// <summary>
        /// 保留wathcer的引用，避免被回收引起监控的中断
        /// </summary>
        private static List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();


        public static void Start()
        {
            if (!_startWasCalled)
            {
                _startWasCalled = true;
                //此行代码注册了WebPageHttpModule
                DynamicModuleUtility.RegisterModule(typeof(WebPageHttpModule));

#if (DEBUG)
                _watchers.Add(PageUtil.WatchFiles(".resx", (file, virtualPath) =>
                {
                    LanguageResources.OnChange(virtualPath);
                }));
#endif

            }
        }
    }
}
