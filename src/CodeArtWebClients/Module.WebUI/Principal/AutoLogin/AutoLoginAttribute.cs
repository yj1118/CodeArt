using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using CodeArt.AOP;
using CodeArt.Common;

namespace Module.WebUI
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class ,Inherited = true, AllowMultiple = false)]
    public class AutoLoginAttribute : AspectAttribute
    {
        public AutoLoginAttribute(bool must = true)
            : base(typeof(AutoLoginAspect), new object[] { must })
        {

        }
    }
}