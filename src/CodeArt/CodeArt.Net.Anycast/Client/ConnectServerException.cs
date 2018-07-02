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
    public class ConnectServerException : Exception
    {
        public IPEndPoint ServerEndPoint
        {
            get;
            private set;
        }

        public ReconnectArgs ReconnectArgs
        {
            get;
            private set;
        }

        public ConnectServerException(IPEndPoint serverEndPoint, Exception ex, ReconnectArgs reconnectArgs)
            : base(Strings.ConnectServerError, ex)
        {
            this.ServerEndPoint = serverEndPoint;
            this.ReconnectArgs = reconnectArgs;
        }
    }
}
