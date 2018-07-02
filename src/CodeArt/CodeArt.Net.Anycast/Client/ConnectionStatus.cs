using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public enum ConnectionStatus : byte
    {
        /// <summary>
        /// 正在连接
        /// </summary>
        Connecting = 1,
        /// <summary>
        /// 已连接
        /// </summary>
        Connected = 2,
        /// <summary>
        /// 已断开
        /// </summary>
        Disconnected = 3
    }
}
