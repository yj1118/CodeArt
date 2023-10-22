using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Concurrent;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 每一个RtpCapability都具备以下特性：
    /// 1.依赖于rtp通道接收和发送数据
    /// 2.具备处理接收到的数据的能力
    /// 3.根据通道的类型不同，可以发送单播或组播数据，仅且只能发送其中一种
    /// </summary>
    public abstract class RtpCapability : IDisposable
    {
        public RtpChannel Channel
        {
            get;
            private set;
        }

        public RtpContext Context
        {
            get
            {
                return this.Channel.Context;
            }
        }


        public AnycastClient Client
        {
            get
            {
                return this.Channel.Context.Client;
            }
        }

        public virtual string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        private void ValidateChannel()
        {
            if (this.Channel == null) throw new InvalidOperationException(string.Format(Strings.ChannelExistsOnCapability, this.Name));
        }


        #region 加入通道

        internal void Join(RtpChannel channel)
        {
            if (this.Channel != null) throw new InvalidOperationException(string.Format(Strings.HasJoinedChannel, this.Name));
            this.Channel = channel;
            HandleJoinedChannel(channel);
        }

        /// <summary>
        /// 处理加入通道
        /// </summary>
        /// <param name="channel"></param>
        protected virtual void HandleJoinedChannel(RtpChannel channel) { }


        /// <summary>
        /// 离开通道
        /// </summary>
        internal void Leave()
        {
            ValidateChannel();
            var channel = this.Channel;
            this.Channel = null;
            HandleLeftChannel(channel);
        }

        /// <summary>
        /// 处理离开通道
        /// </summary>
        /// <param name="channel"></param>
        protected virtual void HandleLeftChannel(RtpChannel channel) { }

        #endregion

        public RtpCapability()
        {
        }

        ///// <summary>
        ///// 获得所在组播的信息
        ///// </summary>
        ///// <returns></returns>
        //public Multicast GetMulticast()
        //{
        //    return this.Channel.Context.Client.GetMulticast(this.Channel.HostAddress);
        //}

        internal static void Process(AnycastClient client, ClientEvents.MessageReceivedEventArgs ea)
        {
            var data = RtpDataAnalyzer.Deserialize(ea.Message.Origin, ea.Message.Body);
            if (TryProcessCall(ea.Message, data)) return;

            var header = ea.Message.Header;
            var origin = ea.Message.Origin;
            var eventName = header.GetValue<string>(FieldRtpEventName);
            RaiseLocalEvent(client, origin, eventName, data);
        }

        /// <summary>
        /// 收到触发rtp事件的事件，该事件是全局的
        /// </summary>
        public static event ReceivedRaiseEventHandler ReceivedRaise;

        private static void RaiseLocalEvent(AnycastClient client, string origin, string eventName, RtpData data)
        {
            if (ReceivedRaise != null)
            {
                var ea = new ReceivedRaiseEventArgs(origin, eventName, data);
                ReceivedRaise(client, ea);
            }
        }

        #region 网络事件

        /// <summary>
        /// 向通道内的所有成员触发事件
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        protected Future<bool> RaiseEvent(string eventName, DTObject header, byte[] body)
        {
            return RaiseEvent(eventName, header, body, this.Channel.HostAddress);
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        protected Future<bool> RaiseEvent(string eventName)
        {
            return RaiseEvent(eventName, DTObject.Empty, Array.Empty<byte>(), this.Channel.HostAddress);
        }

        protected Future<bool> RaiseEvent(string eventName, string destination)
        {
            return RaiseEvent(eventName, DTObject.Empty, Array.Empty<byte>(), destination);
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        protected Future<bool> RaiseEvent(string eventName, DTObject header)
        {
            return RaiseEvent(eventName, header, Array.Empty<byte>(), this.Channel.HostAddress);
        }

        protected Future<bool> RaiseEvent(string eventName, DTObject header,string destination)
        {
            return RaiseEvent(eventName, header, Array.Empty<byte>(), destination);
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="destination"></param>
        protected Future<bool> RaiseEvent(string eventName, DTObject header, byte[] body, string destination)
        {
            return RaiseEvent(eventName, header, body, (mh) =>
            {
                mh.SetValue(MessageField.Destination, destination);
            });
        }

        protected Future<bool> RaiseEvent(string eventName, IEnumerable<string> destinations)
        {
            return RaiseEvent(eventName, DTObject.Empty, Array.Empty<byte>(), destinations);
        }

        protected Future<bool> RaiseEvent(string eventName, DTObject header, IEnumerable<string> destinations)
        {
            return RaiseEvent(eventName, header, Array.Empty<byte>(), destinations);
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="destinations"></param>
        protected Future<bool> RaiseEvent(string eventName, DTObject header, byte[] body, IEnumerable<string> destinations)
        {
            return RaiseEvent(eventName, header, body, (mh) =>
            {
                mh.SetValue(MessageField.Destinations, destinations);
            });
        }

        private Future<bool> RaiseEvent(string eventName, 
                                DTObject header, 
                                byte[] body, 
                                Action<DTObject> setMessageHeader)
        {
            var rtpData = new RtpData(this.Channel.Participant, header, body);
            var data = RtpDataAnalyzer.Serialize(rtpData);

            Message msg = CreateMessage(eventName, data, setMessageHeader);
            return this.Channel.Context.Client.Send(msg);
        }

        private Message CreateMessage(string eventName, byte[] data, Action<DTObject> setMessageHeader)
        {
            var messageHeader = DTObject.Create();
            messageHeader.SetValue(FieldRtpCapabilityName, this.Name);
            messageHeader.SetValue(FieldRtpEventName, eventName);
            messageHeader.SetValue(MessageField.MessageType, (byte)MessageType.Distribute);
            setMessageHeader(messageHeader);

            return new Message(messageHeader, data);
        }

        public static Message CreateMessage(string capabilityName, string eventName, Participant participant, DTObject header, byte[] body, Action<DTObject> setMessageHeader = null)
        {
            var rtpData = new RtpData(participant, header, body);
            var data = RtpDataAnalyzer.Serialize(rtpData);

            var msgHeader = DTObject.Create();
            msgHeader.SetValue(FieldRtpCapabilityName, capabilityName);
            msgHeader.SetValue(FieldRtpEventName, eventName);
            msgHeader.SetValue(MessageField.MessageType, (byte)MessageType.Distribute);
            if(setMessageHeader != null)
                setMessageHeader(msgHeader);

            return new Message(msgHeader, data);
        }

        #endregion

        #region 远程调用

        /// <summary>
        /// 尝试以远程调用的方式处理消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool TryProcessCall(Message msg, RtpData data)
        {
            var requestId = msg.Header.GetValue<Guid>(FieldRtpCallRequestId, Guid.Empty);
            if (requestId == Guid.Empty) return false;

            var error = msg.Header.GetValue<string>(FieldRtpCallErrorMessage, string.Empty);
            var isCompleted = msg.Header.GetValue<bool>(FieldRtpCallIsCompleted, false);

            var identity = RtpCallManager.GetIdentity(requestId);
            if (identity != null)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    //错误处理
                    identity.Future.SetError(new ApplicationException(error));
                }
                else
                {
                    identity.Process(data, isCompleted);
                }

                if (isCompleted)
                {
                    identity.Future.SetResult(true);
                    RtpCallManager.ReturnIdentity(identity);
                }
            }
            return true;
        }



        /// <summary>
        /// 远程调用，该调用非常强大，不仅可以同步或异步执行并且获取结果，更可以获取服务器端多次推送的结果
        /// </summary>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="future"></param>
        private Future<bool> Call(string eventName, DTObject header, byte[] body, Action<RtpData, bool> process, Action<DTObject> setMessageHeader)
        {
            var identity = RtpCallManager.BorrowIdentity();
            identity.Process = process;

            var rtpData = new RtpData(this.Channel.Participant, header, body);
            var data = RtpDataAnalyzer.Serialize(rtpData);

            identity.Future.Start();
            Message msg = CreateCallMessage(this.Name, eventName, data, identity.RequestId, setMessageHeader);
            this.Channel.Context.Client.Send(msg);
            return identity.Future;
        }

        private Future<bool> Call(string eventName, DTObject header, byte[] body, Action<RtpData, bool> process, string destination)
        {
            return Call(eventName, header, body, process, (mh) =>
             {
                 mh.SetValue(MessageField.Destination, destination);
             });
        }

        private Future<bool> Call(string eventName, DTObject header, byte[] body, Action<RtpData, bool> process, IEnumerable<string> destinations)
        {
            return Call(eventName, header, body, process, (mh) =>
            {
                mh.SetValue(MessageField.Destinations, destinations);
            });
        }

        protected Future<bool> Call(string eventName, DTObject header, Action<RtpData, bool> process)
        {
            return Call(eventName, header, Array.Empty<byte>(), process, (mh) =>
            {
                mh.SetValue(MessageField.Destination, this.Channel.HostAddress);
            });
        }

        protected Future<bool> Call(string eventName, DTObject header, byte[] content, Action<RtpData, bool> process)
        {
            return Call(eventName, header, content, process, (mh) =>
            {
                mh.SetValue(MessageField.Destination, this.Channel.HostAddress);
            });
        }

        protected Future<bool> Call(string eventName, DTObject header, Action<RtpData, bool> process, string destination)
        {
            return Call(eventName, header, Array.Empty<byte>(), process, (mh) =>
            {
                mh.SetValue(MessageField.Destination, destination);
            });
        }

        protected Future<bool> Call(string eventName, DTObject header, Action<RtpData, bool> process, IEnumerable<string> destinations)
        {
            return Call(eventName, header, Array.Empty<byte>(), process, (mh) =>
            {
                mh.SetValue(MessageField.Destinations, destinations);
            });
        }

        internal static Message CreateCallMessage(string capabilityName, string eventName, byte[] data, Guid requestId, Action<DTObject> setMessageHeader)
        {
            var header = DTObject.Create();
            header.SetValue(FieldRtpCapabilityName, capabilityName);
            header.SetValue(FieldRtpEventName, eventName);
            header.SetValue(FieldRtpCallRequestId, requestId);
            header.SetValue(MessageField.MessageType, (byte)MessageType.Custom);
            setMessageHeader(header);
            return new Message(header, data);
        }

        #endregion

        #region 静态

        internal const string FieldRtpCapabilityName = "RCN"; //能力名称

        internal const string FieldRtpEventName = "REN"; //事件名称

        internal const string FieldRtpCallRequestId= "RCRI"; //rtp请求的通信编号

        internal const string FieldRtpCallErrorMessage = "RCEM"; //rtp远程调用时遇到了错误的信息

        internal const string FieldRtpCallIsCompleted = "RCIC"; //rtp远程调用已结束

        public static string GetCapabilityName(Message message)
        {
            return message.Header.GetValue<string>(FieldRtpCapabilityName);
        }

        #endregion

        public virtual void Dispose() { }
    }
}
