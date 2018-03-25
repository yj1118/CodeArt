using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.Net.Anycast
{
    public class AnycastEventsBase
    {
        /// <summary>
        /// 需要2个对象
        /// 1) sender;
        /// 2) EventArgs ea;
        /// </summary>
        internal delegate void RaiseEvent(object[] args);

  
        #region 发生异常时触发

        public class ErrorEventArgs : EventArgs
        {
            public Exception Exception
            {
                get;
                private set;
            }
            
            public ErrorEventArgs(Exception exception)
            {
                this.Exception = exception;
            }
        }

        public delegate void ErrorEventHandler(object sender, ErrorEventArgs ea);

        public static event ErrorEventHandler Error;

        private static void _RaiseError(object[] args)
        {
            FireEvent(Error, args);
        }

        /// <summary>
        /// 异步触发客户端已连接的事件
        /// </summary>
        /// <param name="data"></param>
        internal static void AsyncRaiseError(object sender, Exception ex)
        {
            object[] args = { sender, new ErrorEventArgs(ex) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseError), args);
        }


        #endregion

      
        /// <summary>
        /// 该方法用于实际触发事件
        /// 如果成功调用委托那么返回true，否则false(事件没有被挂载)
        /// </summary>
        /// <param name="del"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static bool FireEvent(Delegate del, object[] args)
        {
            bool result = false;

            if (del != null)
            {
                Delegate[] sinks = del.GetInvocationList();

                foreach (Delegate sink in sinks)
                {
                    try
                    {
                        sink.DynamicInvoke(args);
                    }
                    catch (Exception e)
                    {
                        AsyncRaiseError(args[0], e);
                    }
                }
                result = true;
            }

            return result;
        }
    }
}
