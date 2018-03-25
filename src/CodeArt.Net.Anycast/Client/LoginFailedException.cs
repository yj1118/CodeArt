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
    /// 客户端登录认证失败
    /// </summary>
    public class LoginFailedException : Exception
    {
        public LoginFailedException(string message)
            : base(message)
        {
        }
    }
}
