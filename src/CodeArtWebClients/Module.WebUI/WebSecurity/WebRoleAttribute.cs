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
    /// 基于角色验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class WebRoleAttribute : AspectAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspectType">关注点的实现</param>
        public WebRoleAttribute(params string[] roles)
            : base(typeof(WebRole), new object[] { roles })
        {

        }



    }
}
