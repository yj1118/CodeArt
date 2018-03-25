using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

using CodeArt.DTO;
using DotNetty.Handlers.Timeout;

namespace CodeArt.Net.Anycast
{
    internal class HeartBeatRespHandler : ServerHandlerBase
    {
        public HeartBeatRespHandler(AnycastServer server)
            : base(server)
        {
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            SafeProcess(() =>
            {
                var msg = (Message)message;
                if (msg.Type == MessageType.HeartBeatRequest)
                {
                    //响应心跳请求
                    context.WriteAndFlushAsync(_heartBeatMsg);
                    ServerEvents.AsyncRaiseHeartBeatReceived(this.Server, this.Sessions.GetSession(context.Channel));
                }
                else
                    context.FireChannelRead(msg);
            }, context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            SafeProcess(() =>
            {
                if (exception is ReadTimeoutException)
                {
                    return; //心跳超时直接返回，不再提示错误，因为已经离线了
                }
                context.FireExceptionCaught(exception);
            }, context);
        }


        private static readonly Message _heartBeatMsg = null;

        static HeartBeatRespHandler()
        {
            _heartBeatMsg = BuildHeartBeat();
        }

        private static Message BuildHeartBeat()
        {
            var header = DTObject.Create();
            header.SetValue(MessageField.MessageType, (byte)MessageType.HeartBeatResponse);
            return new Message(header, Array.Empty<byte>());
        }

    }
}