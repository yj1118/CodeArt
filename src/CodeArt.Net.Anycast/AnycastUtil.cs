using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

using CodeArt.DTO;
using System.Net.Sockets;
using DotNetty.Buffers;

namespace CodeArt.Net.Anycast
{
    internal static class Util
    {
        public static readonly string RemoveDestination = "!Destination";

        public static readonly string RemoveDestinations = "!Destinations";


        /// <summary>
        /// 异常是否为对方离线导致的，如果是，那么会进行处理，并返回true
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool OfflineHandle(IChannelHandlerContext context, Exception exception)
        {
            var socketException = exception as SocketException;
            if (socketException != null)
            {
                if (socketException.ErrorCode == (int)SocketError.ConnectionReset)
                {
                    //这是客户端被关闭时触发的异常，直接离线即可，不必触发错误事件
                    context.FireExceptionCaught(exception);
                    return true;
                }
            }
            return false;
        }

        public static byte[] ReadBytesToArray(this IByteBuffer buffer, int length)
        {
            var bytes = new byte[length];
            buffer.ReadBytes(bytes, 0, length);
            return bytes;
        }

    }
}
