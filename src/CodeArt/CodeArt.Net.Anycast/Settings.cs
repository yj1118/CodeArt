using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    internal static class Settings
    {
        /// <summary>
        /// 心跳时间间隔（秒）
        /// </summary>
        public const int HeartBeat = 5;

        /// <summary>
        /// 如果超过该时间没有发生任何通讯，那么断开连接（秒）
        /// </summary>
        public const int Timeout = 20;

    }
}