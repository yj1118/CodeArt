using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DotNetty.Buffers;
using DotNetty.Transport.Channels;

using CodeArt.DTO;
using System.Net.Sockets;

namespace CodeArt.Net.Anycast
{

    internal class ClientLogicHandler : ClientHandlerBase
    {
        public ClientLogicHandler(AnycastClient client)
            : base(client)
        {
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            SafeProcess(() =>
            {
                var msg = (Message)message;
                Process(msg);
                this.Client.Process(msg);
            }, context);
        }

        private void Process(Message message)
        {
            switch (message.Type)
            {
                case MessageType.Join:
                    {
                        HandleParticipantUpdate(message);
                        SendIAmHere(message);
                    }
                    break;
                case MessageType.ParticipantUpdated:
                case MessageType.IAmHere:
                    HandleParticipantUpdate(message);
                    break;
                case MessageType.Leave:
                    HandleParticipantLeave(message);
                    break;
                case MessageType.IAmNotHere:
                    HandleParticipantLeave(message);
                    break;
                case MessageType.Custom:
                case MessageType.Distribute:
                    {
                        ReceivedMessage(message);
                    }
                    break;
            }
        }

        private void HandleParticipantUpdate(Message message)
        {
            var participant = DataAnalyzer.DeserializeParticipant(message.Origin, message.Body);
            var multicastAddress = message.Header.GetValue<string>(MessageField.MulticastAddress);
            this.Client.UseMulticast(multicastAddress,(multicast) =>
            {
                participant.Orgin = message.Origin;
                multicast.AddOrUpdateParticipant(participant);
            });           
        }

        private void HandleParticipantLeave(Message message)
        {
            if(message.Body.Length > 0)
            {
                //根据成员删除
                var participant = DataAnalyzer.DeserializeParticipant(message.Origin, message.Body);
                var multicastAddress = message.Header.GetValue<string>(MessageField.MulticastAddress);
                this.Client.UseMulticast(multicastAddress, (multicast) =>
                {
                    multicast.RemoveParticipant(participant);
                });                
            }
            else
            {
                //根据发送源更新成员，这意味着对方掉线了
                var multicastAddress = message.Header.GetValue<string>(MessageField.MulticastAddress);
                this.Client.UseMulticast(multicastAddress, (multicast) =>
                {
                    multicast.RemoveParticipant(message.Origin);
                });
            }
        }

        private void ReceivedMessage(Message message)
        {
            //触发接收消息的事件
            ClientEvents.AsyncRaiseMessageReceived(this.Client, message);
        }


        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            SafeProcess(() =>
            {
                if (Util.OfflineHandle(context, exception)) return;

                ClientEvents.AsyncRaiseError(this.Client, exception);
                context.FireExceptionCaught(exception);
            }, context);
        }

        #region 收到对方加入组播请求后，发送一个 i'm here 消息

        private void SendIAmHere(Message message)
        {
            var multicastAddress = message.Header.GetValue<string>(MessageField.MulticastAddress);
            this.Client.UseMulticast(multicastAddress, (multicast) =>
            {
                var msg = CreateIAmHereMessage(multicast, message.Origin);
                this.Client.Send(msg);
            });
        }

        private Message CreateIAmHereMessage(Multicast multicast, string destination)
        {
            DTObject header = DTObject.Create();
            header.SetValue(MessageField.MessageType, (byte)MessageType.IAmHere);
            header.SetValue(MessageField.MulticastAddress, multicast.Address);
            header.SetValue(MessageField.Destination, destination);
            return new Message(header, multicast.Host.Data);
        }

        #endregion

    }
}