using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.EasyMQ.RPC
{
    public static class RPCEvents
    {
        #region RPC服务器已打开的事件

        /// <summary>
        /// RPC服务器已打开的事件
        /// </summary>
        public class ServerOpenedArgs : EventArgs
        {
            public string MethodName
            {
                get;
                private set;
            }

            public ServerOpenedArgs(string methodName)
            {
                this.MethodName = methodName;
            }
        }

        public delegate void ServerOpenedHandler(object sender, ServerOpenedArgs arg);

        public static event ServerOpenedHandler ServerOpened;

        internal static void RaiseServerOpened(object sender, ServerOpenedArgs arg)
        {
            if (ServerOpened != null)
            {
                ServerOpened(sender, arg);
            }
        }

        #endregion

        #region RPC服务器已关闭的事件

        /// <summary>
        /// RPC服务器已关闭的事件
        /// </summary>
        public class ServerClosedArgs : EventArgs
        {
            public string MethodName
            {
                get;
                private set;
            }

            public ServerClosedArgs(string methodName)
            {
                this.MethodName = methodName;
            }
        }

        public delegate void ServerClosedHandler(object sender, ServerClosedArgs arg);

        public static event ServerClosedHandler ServerClosed;

        internal static void RaiseServerClosed(object sender, ServerClosedArgs arg)
        {
            if (ServerClosed != null)
            {
                ServerClosed(sender, arg);
            }
        }

        #endregion


        #region RPC服务器已关闭的事件

        /// <summary>
        /// RPC服务器已关闭的事件
        /// </summary>
        public class ServerErrorArgs : EventArgs
        {
            public Exception Exception
            {
                get;
                private set;
            }

            public ServerErrorArgs(Exception exception)
            {
                this.Exception = exception;
            }
        }

        public delegate void ServerErrorHandler(object sender, ServerErrorArgs arg);

        public static event ServerErrorHandler ServerError;

        public static void RaiseServerError(object sender, ServerErrorArgs arg)
        {
            if (ServerError != null)
            {
                ServerError(sender, arg);
            }
        }

        #endregion

    }
}
