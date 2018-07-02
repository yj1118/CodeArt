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
    /// 连接服务器错误
    /// </summary>
    public class NetworkException : Exception
    {
        public NetworkException()
            : base(Strings.ConnectServerError)
        {
        }
    }
}
