using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.CodeDom;
using System.IO;
using System.Reflection;

using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml
{
    internal static class XPCAssembly
    {
        public static bool Exists(string virtualPath)
        {
            return XPCAssemblyLoader.Create().Exists(virtualPath);
        }

        /// <summary>
        /// 编译程序集
        /// </summary>
        /// <param name="coder"></param>
        public static void Compile(string virtualPath)
        {
            XPCAssemblyLoader.Create().Compile(virtualPath);
        }

        /// <summary>
        /// 加载xaml页面组件的程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly Load(string virtualPath)
        {
            return XPCAssemblyLoader.Create().Load(virtualPath);
        }

        
        /// <summary>
        /// 通过虚拟路径，删除程序集
        /// </summary>
        /// <param name="virtualPath"></param>
        public static void Delete(string virtualPath)
        {
            XPCAssemblyLoader.Create().Delete(virtualPath);
        }

    }


}
