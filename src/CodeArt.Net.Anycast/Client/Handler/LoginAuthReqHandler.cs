using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

using DotNetty.Buffers;
using DotNetty.Transport.Channels;

using CodeArt.DTO;

namespace CodeArt.Net.Anycast
{

    internal class LoginAuthReqHandler : ClientHandlerBase
    {
        public LoginAuthReqHandler(AnycastClient client)
            : base(client)
        {
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            SafeProcess(() =>
            {
                //通道激活后发送认证请求消息
                context.WriteAndFlushAsync(BuildLoginReq());
            }, context);
        }

        private Message BuildLoginReq()
        {
            var header = DTObject.Create();
            header.SetValue(MessageField.MessageType, MessageType.LoginRequest);
            header.SetObject(MessageField.LoginIdentity, this.Client.Config.IdentityProvider.GetIdentity());

            return new Message(header, Array.Empty<byte>());
        }


        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            SafeProcess(() =>
            {
                var msg = (Message)message;
                if (msg.Type == MessageType.LoginResponse)
                {
                    var result = CertifiedResult.FromMessage(msg);
                    if (result.IsOK)
                    {
                        //认证成功
                        Online(context);
                        context.FireChannelRead(msg);
                    }
                    else
                    {
                        //认证失败
                        ClientEvents.AsyncRaiseError(this.Client, new LoginFailedException(result.Info));
                        context.CloseAsync();
                    }
                }
                else
                {
                    //不是登录认证的响应，那么交由下一个通道处理器
                    context.FireChannelRead(msg);
                }
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

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            SafeProcess(() =>
            {
                Offline(context);
                context.FireExceptionCaught(exception);
            }, context);

        }

    }
}