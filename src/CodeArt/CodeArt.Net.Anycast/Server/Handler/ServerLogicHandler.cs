using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;

using DotNetty.Buffers;
using DotNetty.Transport.Channels;

using CodeArt.DTO;
using CodeArt.Concurrent.Pattern;


namespace CodeArt.Net.Anycast
{

    internal class ServerLogicHandler : ServerHandlerBase
    {
        public ServerLogicHandler(AnycastServer server)
            : base(server)
        {
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            SafeProcess(() =>
            {
                var msg = (Message)message;
                //补充来源信息
                msg.Origin = RemoteSession.GetUnicastAddress(context.Channel);

                //找到发送者在服务器端的回话对象
                var senders = this.Sessions.GetSessions(msg.Origin);
                foreach (var sender in senders)
                {
                    using (var temp = HandlerContext.Pool.Borrow())
                    {
                        var ctx = temp.Item;
                        if (sender.IsActive)
                            this.Server.StartProcess(sender, msg, ctx);

                        if (!ctx.IsCompleted && sender.IsActive)
                            Process(sender, msg, ctx);

                        //使用服务器扩展的handler处理
                        if (sender.IsActive)
                            this.Server.EndProcess(sender, msg, ctx);
                    }
                }
            }, context);
        }

        #region 自带的处理器

        private void Process(IServerSession origin, Message message, HandlerContext ctx)
        {
            switch (message.Type)
            {
                case MessageType.Join:
                    Join(origin, message);
                    break;
                case MessageType.Leave:
                    Leave(origin, message);
                    break;
                case MessageType.ParticipantUpdated:
                case MessageType.IAmHere:
                case MessageType.Distribute:
                    Distribute(origin, message);
                    break;
            }
        }

        #endregion

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            SafeProcess(() =>
            {
                Offline(context);

                if (Util.OfflineHandle(context, exception)) return;

                ServerEvents.AsyncRaiseError(this.Server, exception);
                context.FireExceptionCaught(exception);
            }, context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            SafeProcess(() =>
            {
                Offline(context);
                context.FireChannelInactive();
            }, context);
        }


     


    }
}