using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

using CodeArt.DTO;
using DotNetty.Handlers.Timeout;

namespace CodeArt.Net.Anycast
{
    internal class LoginAuthRespHandler : ServerHandlerBase
    {
        public LoginAuthRespHandler(AnycastServer server)
            : base(server)
        {
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            SafeProcess(() =>
            {
                var msg = (Message)message;
                if (IsLoginRequest(msg))
                {
                    var identity = msg.Header.GetObject(MessageField.LoginIdentity);
                    var result = Server.Config.Authenticator.Check(identity);
                    context.WriteAndFlushAsync(BuildLoginResp(result));
                    if (result.IsOK)
                    {
                        Online(context);
                    }
                }
                else
                {
                    //不是登录认证的请求，那么交由下一个通道处理器
                    context.FireChannelRead(msg);
                }
            }, context);
        }

        private bool IsLoginRequest(Message msg)
        {
            return msg.Type == MessageType.LoginRequest;
        }

        private Message BuildLoginResp(CertifiedResult result)
        {
            return result.ToMessage();
        }
    }
}