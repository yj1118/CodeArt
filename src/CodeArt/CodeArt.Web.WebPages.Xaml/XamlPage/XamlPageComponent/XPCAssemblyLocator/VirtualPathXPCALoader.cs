using System;
using System.Linq;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using System.Text;
using System.IO;

using CodeArt.AOP;
using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]
    public class VirtualPathXPCALocator : IXPCAssemblyLoader
    {
        private Func<string, Assembly> _getAssembly;

        protected VirtualPathXPCALocator()
        {
            _getAssemblyFileName = LazyIndexer.Init<string, string>(GetAssemblyFileNameImpl);
            _getAssembly = LazyIndexer.Init<string, Assembly>(LoadAssemblyImpl);
        }

        protected virtual Assembly LoadAssemblyImpl(string virtualPath)
        {
            var assemblyFileName = GetAssemblyFileName(virtualPath);
            if (!File.Exists(assemblyFileName))
            {
                Compile(virtualPath);
            }
            if (!File.Exists(assemblyFileName)) return null;
            return Assembly.LoadFrom(assemblyFileName);
        }




        private Func<string, string> _getAssemblyFileName;

        protected static Func<string, string> _getAssemblyName = LazyIndexer.Init<string, string>((virtualPath) =>
        {
            var path = virtualPath.TrimStart("/");
            int pos = path.LastIndexOf(".");
            if (pos > 0) path = path.Substring(0, pos);
            path = path.Replace("/", "_");
            return path;
        });

        protected virtual string GetAssemblyFileNameImpl(string virtualPath)
        {
            var assemblyName = _getAssemblyName(virtualPath);
            string path = string.Format("{0}.dll", assemblyName);
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bin", "$App_Code", path);
        }

        protected string GetAssemblyFileName(string virtualPath)
        {
            return _getAssemblyFileName(virtualPath);
        }

        public Assembly Load(string virtualPath)
        {
            return _getAssembly(virtualPath);
        }

        public bool Exists(string virtualPath)
        {
            var assemblyFileName = GetAssemblyFileName(virtualPath);
            return File.Exists(assemblyFileName);
        }

        private static object _syncObject = new object();

        /// <summary>
        /// 删除已生成的程序集
        /// </summary>
        private void DeleteAssembly(string assemblyFileName)
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
        public void Delete(string virtualPath)
        {
            var assemblyFileName = GetAssemblyFileName(virtualPath);
            DeleteAssembly(assemblyFileName);
        }

        private static string GetPDB(string assemblyFileName)
        {
            var temp = assemblyFileName.Remove(assemblyFileName.Length - 3, 3);
            return string.Format("{0}pdb", temp);
        }

        /// <summary>
        /// 编译程序集
        /// </summary>
        /// <param name="coder"></param>
        public void Compile(string virtualPath)
        {
            var assemblyFileName = GetAssemblyFileName(virtualPath);
            DeleteAssembly(assemblyFileName);

            var coder = XPCCoder.Create(virtualPath);
            //只有从文件编译才能调试
            CodeCompiler.CompileCSharpFromFile(assemblyFileName, new string[] { coder.UserPath, coder.GIPath, coder.GPath },new string[] { "Microsoft.CSharp.dll" });
        }

        //protected virtual string[] GetSourceCodes(string virtualPath)
        //{
        //    //生成程序集
        //    var coder = XPCCoder.Create(virtualPath);
        //    if (coder == null) return Array.Empty<string>();
        //    return GetSourceCodesImpl(coder);
        //}

        //protected virtual string[] GetSourceCodesImpl(XPCCoder coder)
        //{
        //    var codes = new string[3];
        //    codes[0] = File.ReadAllText(coder.UserPath);
        //    codes[1] = File.ReadAllText(coder.GIPath);
        //    codes[2] = File.ReadAllText(coder.GPath);
        //    return codes;
        //}


        public static readonly VirtualPathXPCALocator Instance = new VirtualPathXPCALocator();
    }
}
