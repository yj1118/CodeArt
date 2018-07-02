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
    internal sealed class CSharpWatcher
    {
        private static object _syncObject = new object();

        public void OnChange(string virtualPath)
        {
            if (IsIgnore(virtualPath)) return;

            lock (_syncObject)
            {
                try
                {
                    var htmlVirtualPath = MapHtmlVirtualPath(virtualPath);
                    XPCAssembly.Delete(htmlVirtualPath);//删除程序集
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

        private static string MapHtmlVirtualPath(string csharpVirtualPath)
        {
            return csharpVirtualPath.Replace("App_Code/", string.Empty).Replace(".cs", ".htm");
        }


        /// <summary>
        /// 如果路径中含有$忽略
        /// 如果是生成的g目录也忽略
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        private static bool IsIgnore(string virtualPath)
        {
            return virtualPath.IndexOf("$") > -1 || virtualPath.IndexOf("App_Code/g") > -1;
        }


        public readonly static CSharpWatcher Instance = new CSharpWatcher();
    }


}
