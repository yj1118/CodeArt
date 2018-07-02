using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Net;

using CodeArt.Util;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 运行服务器出错
    /// </summary>
    public class RunServerException : Exception
    {
        public RunServerException(Exception ex)
            : base(Strings.RunServerError, ex)
        {
        }
    }
}
