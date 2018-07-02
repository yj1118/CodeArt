using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;

using CodeArt.DTO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace CodeArt.Net.Anycast
{
    public enum MessageType : byte
    {
        Unspecified = 0,

        /// <summary>
        /// 登录请求
        /// </summary>
        LoginRequest = 1,
        /// <summary>
        /// 登录响应
        /// </summary>
        LoginResponse = 2,

        /// <summary>
        /// 心跳请求
        /// </summary>
        HeartBeatRequest = 3,

        /// <summary>
        /// 心跳响应
        /// </summary>
        HeartBeatResponse = 4,

        /// <summary>
        /// 加入组播
        /// </summary>
        Join = 5,

        /// <summary>
        /// 离开组播
        /// </summary>
        Leave = 6,

        /// <summary>
        /// 转发消息
        /// </summary>
        Distribute = 7,

        /// <summary>
        /// 发送一个 i'm here 消息，提示发送方在组播中
        /// </summary>
        IAmHere = 8,

        /// <summary>
        /// 发送一个 i'm not hrer 消息，提示发送方不在组播中
        /// </summary>
        IAmNotHere = 9,

        /// <summary>
        /// 负责人被改变的消息
        /// </summary>
        ParticipantUpdated = 10,

        /// <summary>
        /// 自定义
        /// </summary>
        Custom = 99
    }
}
