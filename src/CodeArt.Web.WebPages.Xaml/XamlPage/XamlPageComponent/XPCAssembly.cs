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
        public static bool Exists(XPCCoder coder)
        {
            var assemblyFileName = _getAssemblyFileName(coder.VirtualPath);
            return File.Exists(assemblyFileName);
        }

        /// <summary>
        /// 编译程序集
        /// </summary>
        /// <param name="coder"></param>
        public static void Compile(XPCCoder coder)
        {
            var assemblyFileName = _getAssemblyFileName(coder.VirtualPath);
            DeleteAssembly(assemblyFileName);
            CodeCompiler.CompileCSharp(assemblyFileName, coder.UserPath, coder.GIPath, coder.GPath);
        }

        private static Func<string, string> _getAssemblyFileName = LazyIndexer.Init<string, string>((virtualPath) =>
        {
            var assemblyName = _getAssemblyName(virtualPath);
            string path = string.Format("{0}.dll", assemblyName);
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "$App_Code", path);
        });

        private static Func<string, string> _getAssemblyName = LazyIndexer.Init<string, string>((virtualPath) =>
        {
            var path = virtualPath.TrimStart("/");
            int pos = path.LastIndexOf(".");
            if (pos > 0) path = path.Substring(0, pos);
            path = path.Replace("/", "_");
            return path;
        });

        private static Func<string, Assembly> _getAssembly = LazyIndexer.Init<string, Assembly>((virtualPath)=>
        {
            var assemblyFileName = _getAssemblyFileName(virtualPath);
            if (!File.Exists(assemblyFileName))
            {
                //生成程序集
                var coder = XPCCoder.Create(virtualPath);
                if (coder == null) return null;
                Compile(coder);
            }
            return Assembly.LoadFrom(assemblyFileName);
            //return AssemblyUtil.LoadWithNoLock(assemblyFileName); 不能用该方法，因为不能调试
        });

        /// <summary>
        /// 加载xaml页面组件的程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly Load(string virtualPath)
        {
            return _getAssembly(virtualPath);
        }

        private static object _syncObject = new object();

        /// <summary>
        /// 删除已生成的程序集
        /// </summary>
        private static void DeleteAssembly(string assemblyFileName)
        {
            //一次只能有一个线程清理文件
            lock (_syncObject)
            {
                if (File.Exists(assemblyFileName))
                    File.Delete(assemblyFileName);

                string pdb = GetPDB(assemblyFileName);
                if (File.Exists(pdb))
                    File.Delete(pdb);
            }
        }

        /// <summary>
        /// 通过虚拟路径，删除程序集
        /// </summary>
        /// <param name="virtualPath"></param>
        public static void Delete(string virtualPath)
        {
            var assemblyFileName = _getAssemblyFileName(virtualPath);
            DeleteAssembly(assemblyFileName);
        }

        private static string GetPDB(string assemblyFileName)
        {
            var temp = assemblyFileName.Remove(assemblyFileName.Length - 3, 3);
            return string.Format("{0}pdb", temp);
        }


    }


}
