using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.CodeDom;
using System.IO;
using System.Reflection;
using System.Configuration;

using HtmlAgilityPack;
using CodeArt.IO;
using CodeArt.Runtime;
using CodeArt.Web.WebPages;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    internal sealed class HtmlWatcher
    {
        private static object _syncObject = new object();

        public void OnChange(string virtualPath)
        {
            lock (_syncObject)
            {
                try
                {
                    var coder = XPCCoder.Create(virtualPath);
                    if (coder == null) return;
                    coder.Generate();
                    XPCAssembly.Delete(coder.VirtualPath);//删除程序集
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //清空缓存
                    XamlUtil.RecoverMemory();
                }
            }
        }



        public static HtmlWatcher Instance = new HtmlWatcher();
    }


}
