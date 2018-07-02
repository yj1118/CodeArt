using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using CodeArt.AOP;

namespace CodeArt.Web.WebPages
{
    [AttributeUsage(AttributeTargets.Class ,Inherited = true, AllowMultiple = false)]
    public class WebCrossDomainAttribute : AspectAttribute
    {
        public WebCrossDomainAttribute()
            : base(typeof(WebCrossDomain), new object[] { })
        {

        }
    }
}
