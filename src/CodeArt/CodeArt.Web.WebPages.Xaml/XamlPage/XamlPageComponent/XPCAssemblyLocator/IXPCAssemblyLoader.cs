using System;
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

namespace CodeArt.Web.WebPages.Xaml
{
    public interface IXPCAssemblyLoader
    {
        bool Exists(string virtualPath);

        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        Assembly Load(string virtualPath);

        void Delete(string virtualPath);

        void Compile(string virtualPath);

    }
}
