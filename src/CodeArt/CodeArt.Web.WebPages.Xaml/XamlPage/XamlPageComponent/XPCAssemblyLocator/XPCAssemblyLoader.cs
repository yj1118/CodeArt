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
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    public static class XPCAssemblyLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="router"></param>
        public static void Register(IXPCAssemblyLoader instance)
        {
            SafeAccessAttribute.CheckUp(instance.GetType());
            _instance = instance;
        }

        public static IXPCAssemblyLoader Create()
        {
            if (_instance == null) return VirtualPathXPCALocator.Instance;
            return _instance;
        }

        private static IXPCAssemblyLoader _instance = null;
    }
}
