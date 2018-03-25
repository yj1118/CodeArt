using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DotNetty.Buffers;
using DotNetty.Transport.Channels;

using CodeArt.DTO;

namespace CodeArt.Net.Anycast
{

    internal class ClientHandlerBase : ChannelHandlerAdapter
    {
        public AnycastClient Client
        {
            get;
            private set;
        }

        public ClientHandlerBase(AnycastClient client)
        {
            this.Client = client;
        }

        protected void SafeProcess(Action action, IChannelHandlerContext context)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Offline(context);
                ClientEvents.AsyncRaiseError(this.Client, ex);
            }
        }


        /// <summary>
        /// 会话上线
        /// </summary>
        /// <param name="session"></param>
        /// <param name="package"></param>
        protected void Online(IChannelHandlerContext ctx)
        {
            this.Client.Status = ConnectionStatus.Connected;
            this.Client.Channel = ctx.Channel;  //原本是不需要再这里设置的，但是在一些很偶然的情况下，
                                                //我们发现通过Channel = bootstrap.ConnectAsync(this.ServerEndPoint).Result;
                                                //会比这里更加晚执行，导致Channel为null
                                                //所以此处手动赋值一次
            ClientEvents.AsyncRaiseConnected(this.Client, this.Client.ServerEndPoint); //触发已连接的事件
        }


        protected void Offline(IChannelHandlerContext ctx)
        {
            this.Client.Dropped();
        }

    }
}