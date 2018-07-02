using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using CodeArt.AOP;

namespace CodeArt.Web.WebPages
{
    [AttributeUsage(AttributeTargets.Class ,Inherited = false, AllowMultiple = false)]
    public class WebRedirectAttribute : AspectAttribute
    {
        public WebRedirectAttribute(string url)
            : base(typeof(WebRedirect), new object[] { url })
        {

        }
    }
}
