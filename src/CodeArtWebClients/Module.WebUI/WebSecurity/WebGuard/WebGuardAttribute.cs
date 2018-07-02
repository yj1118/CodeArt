using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using CodeArt.Util;
using CodeArt.AOP;
using CodeArt.Web.WebPages;

namespace Module.WebUI
{
    /// <summary>
    /// 自定义的验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class WebGuardAttribute : AspectAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guardType">请保证是单例的，并且实现了<typeparamref name="IWebGuard"/>接口</param>
        public WebGuardAttribute(Type guardType)
            : base(typeof(WebGuard), guardType)
        {

        }
    }
}
