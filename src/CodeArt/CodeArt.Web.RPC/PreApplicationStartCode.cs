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
using CodeArt.Web.WebPages;

[assembly: PreApplicationStartMethod(typeof(CodeArt.Web.RPC.PreApplicationStartCode), "Start")]

namespace CodeArt.Web.RPC
{
    public class PreApplicationStartCode
    {
        private static bool _startWasCalled;

        public static void Start()
        {
            if (!_startWasCalled)
            {
                _startWasCalled = true;
                RegisterRPC();
            }
        }

        private static void RegisterRPC()
        {
            WebPageLocatorFactory.RegisterLocator(string.Empty, RPCPageLocator.Instance);
            WebPageExtensions.Register(string.Empty);  //表示可以接受无后缀的文件请求
        }
    }
}
