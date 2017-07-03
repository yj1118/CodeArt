using System;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Threading;
using CodeArt.Log;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;


namespace CodeArt.Concurrent
{
    /// <summary>
    /// 多媒体定时器的类型
    /// </summary>
    public enum MediaTimerMode
    {
        /// <summary>
        /// 只执行一次
        /// </summary>
        OneShot,
        /// <summary>
        /// 循环执行
        /// </summary>
        Periodic
    }
}
