using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace CodeArt.AOP
{
    /// <summary>
    /// 移除切面
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AspectRemoveAttribute : Attribute
    {
        public Type AspectType
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspectType">关注点的实现</param>
        public AspectRemoveAttribute(Type aspectType)
        {
            this.AspectType = aspectType;
        }

        public bool NeedRemove(AspectAttribute attr)
        {
            return this.AspectType.Equals(attr.AspectType);
        }

    }
}
