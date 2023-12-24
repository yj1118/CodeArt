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
    internal interface IXPCFactory
    {
        /// <summary>
        /// 创建与虚拟路径XamlPage相关的组件
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        object CreateComponent(string virtualPath);

        /// <summary>
        /// 得到与虚拟路径XamlPage相关的组件类型
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        Type GetComponentType(string virtualPath);
    }
}
