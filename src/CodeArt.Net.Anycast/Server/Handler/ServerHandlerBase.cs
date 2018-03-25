using System;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

using CodeArt.DTO;
using CodeArt.Util;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CodeArt.Net.Anycast
{
    internal abstract class ServerHandlerBase : ChannelHandlerAdapter
    {
        public AnycastServer Server
        {
            get;
            private set;
        }

        public SessionManager Sessions
        {
            get
            {
                return this.Server.Sessions;
            }
        }


        private Func<string, object> _getSyncObject = LazyIndexer.Init<string, object>((multicastAddress) =>
        {
            return new object();
        });

        /// <summary>
        /// 针对同一个组播，以同步的形式执行action方法
        /// </summary>
        /// <param name="multicastAddress"></param>
        /// <param name="action"></param>
        protected void ExecuteSync(string multicastAddress,Action action)
        {
            var syncObject = _getSyncObject(multicastAddress);
            lock(syncObject)
            {
                action();
            }
        }

        public ServerHandlerBase(AnycastServer server)
        {
            this.Server = server;
        }

        #region 加入、离开组播、分发数据

        /// <summary>
        /// 将会话加入到一个组播
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="message"></param>
        protected void Join(IServerSession origin, Message message)
        {
            var multicastAddress = message.Header.GetValue<string>(MessageField.MulticastAddress);
            this.ExecuteSync(multicastAddress, () =>
            {
                if (this.Sessions.Join(multicastAddress, origin))
                {
                    //向组播成员转发当前会话加入组播的信息
                    Distribute(origin, message, multicastAddress);
                }
            });
        }

        protected void Leave(IServerSession origin, Message message)
        {
            var multicastAddress = message.Header.GetValue<string>(MessageField.MulticastAddress);
            this.ExecuteSync(multicastAddress, () =>
            {
                if (this.Sessions.Leave(multicastAddress, origin))
                {
                    //向组播成员转发当前会话离开组播的信息
                    Distribute(origin, message, multicastAddress);
                }
            });
        }

        /// <summary>
        /// 分发数据
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="msg"></param>
        protected void Distribute(IServerSession origin, Message msg)
        {
            var destination = msg.Header.GetValue<string>(MessageField.Destination, string.Empty);
            if (!string.IsNullOrEmpty(destination))
            {
                msg.Header.Transform(Util.RemoveDestination);
                Distribute(origin, msg, destination);
                return;
            }

            var destinations = msg.Header.GetList(MessageField.Destinations, false)?.ToArray<string>();
            if (destinations != null)
            {
                msg.Header.Transform(Util.RemoveDestinations);
                Distribute(origin, msg, destinations);
                return;
            }

        }

        protected void Distribute(IServerSession origin, Message msg, IEnumerable<string> destinations)
        {
            foreach (var destination in destinations)
            {
                Distribute(origin, msg, destination);
            }
        }

        protected void Distribute(IServerSession origin, Message msg, string destination)
        {
            var sessions = this.Sessions.GetAnySessions(destination);
            Parallel.ForEach(sessions, (target) =>
            {
                if (target.UnicastAddress == origin.UnicastAddress) return;//不发送给自己
                if (!target.IsActive) return;

                var msgCopy = msg.Clone();
                msgCopy.Header.SetValue(MessageField.Destination, destination); //设置目标地址

                target.Process(msgCopy);
            });
        }

        #endregion

        #region  上线和下线

        /// <summary>
        /// 会话上线
        /// </summary>
        /// <param name="session"></param>
        /// <param name="package"></param>
        protected void Online(IChannelHandlerContext ctx)
        {
            var session = new RemoteSession(ctx.Channel);
            this.Sessions.Add(session);
        }

        protected void Offline(IChannelHandlerContext ctx)
        {
            var session = this.Sessions.GetSession(ctx.Channel);
            if (session != null)
            {
                //先退出组播
                var addresses = this.Sessions.GetMulticastAddresses(session);
                foreach (var address in addresses)
                {
                    var msg = CreateIAmNotHereMessage(address);
                    msg.Origin = session.UnicastAddress;
                    Leave(session, msg);
                }
                //再移除回话
                this.Sessions.Remove(session);
            }
        }

        private Message CreateIAmNotHereMessage(string multicastAddress)
        {
            DTObject header = DTObject.Create();
            header.SetValue(MessageField.MessageType, (byte)MessageType.IAmNotHere);
            header.SetValue(MessageField.MulticastAddress, multicastAddress);
            return new Message(header, Array.Empty<byte>());
        }

        #endregion

        protected void SafeProcess(Action action, IChannelHandlerContext context)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                Offline(context);
                ServerEvents.AsyncRaiseError(this.Server, ex);
            }            
        }

    }
}