using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

using CodeArt.Log;

namespace CodeArt.Web.WebPages
{
    [NonLog]
    internal sealed class RedirectException : Exception
    {
        public string Url
        {
            get;
            private set;
        }

        public RedirectException(string url)
            : base("页面跳转")
        {
            this.Url = url;
        }
    }
}
