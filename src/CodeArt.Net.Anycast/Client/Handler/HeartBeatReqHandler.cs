using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DotNetty.Buffers;
using DotNetty.Transport.Channels;

using CodeArt.DTO;
using DotNetty.Handlers.Timeout;

namespace CodeArt.Net.Anycast
{

    internal class HeartBeatReqHandler : ClientHandlerBase
    {
        private CancellationTokenSource _heartBeatCancellation;

        public HeartBeatReqHandler(AnycastClient client)
            : base(client)
        {
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            SafeProcess(() =>
            {
                var msg = (Message)message;
                if (msg.Type == MessageType.LoginResponse)
                {
                    DisposeCancellation();
                    _heartBeatCancellation = new CancellationTokenSource();
                    //得到登录响应后，开始心跳
                    context.Executor.ScheduleAsync(RunHeartBeat, context, _delay, _heartBeatCancellation.Token);
                }
                else if (msg.Type == MessageType.HeartBeatResponse)
                {
                    //得到回复后继续心跳
                    context.Executor.ScheduleAsync(RunHeartBeat, context, _delay, _heartBeatCancellation.Token);

                    ClientEvents.AsyncRaiseHeartBeatReceived(this.Client, this.Client.ServerEndPoint);
                }
                else
                    context.FireChannelRead(message);
            }, context);
        }


        private static void RunHeartBeat(object context)
        {
            var ctx = (IChannelHandlerContext)context;
            ctx.WriteAndFlushAsync(_heartBeatMsg);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            SafeProcess(() =>
            {
                DisposeCancellation();
                context.FireChannelInactive();
            }, context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            SafeProcess(() =>
            {
                DisposeCancellation();

                if (exception is ReadTimeoutException)
                {
                    return; //心跳超时直接返回，不再提示错误，因为已经离线了
                }

                context.FireExceptionCaught(exception);
            }, context);
        }

        private void DisposeCancellation()
        {
            if (_heartBeatCancellation != null)
            {
                _heartBeatCancellation.Cancel(true);
                _heartBeatCancellation.Dispose();
                _heartBeatCancellation = null;
            }
        }


        private static readonly Message _heartBeatMsg = null;
        private static readonly TimeSpan _delay = TimeSpan.FromSeconds(Settings.HeartBeat);

        static HeartBeatReqHandler()
        {
            _heartBeatMsg = BuildHeartBeat();
        }

        private static Message BuildHeartBeat()
        {
            var header = DTObject.Create();
            header.SetValue(MessageField.MessageType, (byte)MessageType.HeartBeatRequest);
            return new Message(header, Array.Empty<byte>());
        }

    }
}