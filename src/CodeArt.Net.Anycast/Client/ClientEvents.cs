using CodeArt.Log;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;


namespace CodeArt.Net.Anycast
{
    public class ClientEvents : AnycastEventsBase
    {
        #region 正在连接服务器

        /// <summary>
        /// 正在连接服务器的事件
        /// </summary>
        public class ConnectingEventArgs : EventArgs
        {
            /// <summary>
            /// 是否为掉线重连
            /// </summary>
            public bool IsReconnect
            {
                get
                {
                    return this.ReconnectArgs != null && this.ReconnectArgs.Times > 0;
                }
            }

            public ReconnectArgs ReconnectArgs
            {
                get;
                private set;
            }


            internal ConnectingEventArgs(ReconnectArgs reconnectArgs)
            {
                this.ReconnectArgs = reconnectArgs;
            }
        }

        public delegate void ConnectingEventHandler(object sender, ConnectingEventArgs ea);

        public static event ConnectingEventHandler Connecting;

        private static void _RaiseConnecting(object[] args)
        {
            FireEvent(Connecting, args);
        }

        internal static void AsyncRaiseConnecting(object sender, ReconnectArgs reconnectArgs)
        {
            if (Connecting == null) return;

            object[] args = { sender, new ConnectingEventArgs(reconnectArgs) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseConnecting), args);
        }


        #endregion

        #region 连接服务器成功

        /// <summary>
        /// 连接服务器成功的事件
        /// </summary>
        public class ConnectedEventArgs : EventArgs
        {
            public IPEndPoint ServerEndPoint
            {
                get;
                private set;
            }

            public ConnectedEventArgs(IPEndPoint serverEndPoint)
            {
                this.ServerEndPoint = serverEndPoint;
            }
        }

        public delegate void ConnectedEventHandler(object sender, ConnectedEventArgs ea);

        public static event ConnectedEventHandler Connected;

        private static void _RaiseConnected(object[] args)
        {
            FireEvent(Connected, args);
        }

        internal static void AsyncRaiseConnected(object sender, IPEndPoint serverEndPoint)
        {
            if (Connected == null) return;

            object[] args = { sender, new ConnectedEventArgs(serverEndPoint) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseConnected), args);
        }

        #endregion

        #region 心跳事件

        /// <summary>
        /// 收到心跳包
        /// </summary>
        public class HeartBeatReceivedEventArgs : EventArgs
        {
            public IPEndPoint ServerEndPoint
            {
                get;
                private set;
            }

            public HeartBeatReceivedEventArgs(IPEndPoint serverEndPoint)
            {
                this.ServerEndPoint = serverEndPoint;
            }
        }

        public delegate void HeartBeatReceivedEventHandler(object sender, HeartBeatReceivedEventArgs ea);

        public static event HeartBeatReceivedEventHandler HeartBeatReceived;

        private static void _RaiseHeartBeatReceived(object[] args)
        {
            FireEvent(HeartBeatReceived, args);
        }

        internal static void AsyncRaiseHeartBeatReceived(object sender, IPEndPoint serverEndPoint)
        {
            if (HeartBeatReceived == null) return;

            object[] args = { sender, new HeartBeatReceivedEventArgs(serverEndPoint) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseHeartBeatReceived), args);
        }

        #endregion

        #region 从服务器断开的事件

        /// <summary>
        /// 从服务器断开的事件，无论是掉线、还是正常退出都会触发断开连接的事件
        /// </summary>
        public class DisconnectedEventArgs : EventArgs
        {
            /// <summary>
            /// 是否因为网络不通、程序异常等原因导致的连接断开
            /// </summary>
            public bool IsDropped
            {
                get;
                private set;
            }

            public DisconnectedEventArgs(bool isDropped)
            {
                this.IsDropped = isDropped;
            }
        }

        public delegate void DisconnectedEventHandler(object sender, DisconnectedEventArgs ea);

        public static event DisconnectedEventHandler Disconnected;

        private static void _RaiseDisconnected(object[] args)
        {
            FireEvent(Disconnected, args);
        }

        internal static void AsyncRaiseDisconnected(object sender, bool isDropped)
        {
            if (Disconnected == null) return;

            object[] args = { sender, new DisconnectedEventArgs(isDropped) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseDisconnected), args);
        }

        #endregion

        #region 多次尝试重连服务器后失败

        /// <summary>
        /// 多次尝试重连服务器后失败
        /// </summary>
        public class ReconnectFailedEventArgs : EventArgs
        {
            public ReconnectFailedEventArgs()
            {
            }
        }

        public delegate void ReconnectFailedEventHandler(object sender, ReconnectFailedEventArgs ea);

        /// <summary>
        /// 多次尝试重连服务器后失败
        /// </summary>
        public static event ReconnectFailedEventHandler ReconnectFailed;

        private static void _ReconnectFailed(object[] args)
        {
            FireEvent(ReconnectFailed, args);
        }

        internal static void AsyncRaiseReconnectFailed(object sender)
        {
            if (ReconnectFailed == null) return;

            object[] args = { sender, new ReconnectFailedEventArgs() };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_ReconnectFailed), args);
        }

        #endregion

        /// <summary>
        /// 组播参与人的事件参数
        /// </summary>
        public class ParticipantEventArgs : EventArgs
        {
            public Multicast Multicast
            {
                get;
                private set;
            }

            public Participant Participant
            {
                get;
                private set;
            }

            /// <summary>
            /// 负责人是否为本地的
            /// </summary>
            public bool IsLocal
            {
                get;
                private set;
            }

            public ParticipantEventArgs(Multicast multicast, Participant participant,bool isLocal)
            {
                this.Multicast = multicast;
                this.Participant = participant;
                this.IsLocal = isLocal;
            }
        }


        #region 新的组播成员加入的事件

        public delegate void ParticipantAddedEventHandler(object sender, ParticipantEventArgs ea);

        public static event ParticipantAddedEventHandler ParticipantAdded;

        private static void _RaiseParticipantAdded(object[] args)
        {
            FireEvent(ParticipantAdded, args);
        }

        internal static void AsyncRaiseParticipantAdded(object sender, Multicast multicast, Participant participant,bool isLocal)
        {
            if (ParticipantAdded == null) return;

            object[] args = { sender, new ParticipantEventArgs(multicast, participant, isLocal) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseParticipantAdded), args);
        }

        #endregion

        #region 组播成员数据被更改的事件

        public delegate void ParticipantUpdatedEventHandler(object sender, ParticipantEventArgs ea);

        public static event ParticipantUpdatedEventHandler ParticipantUpdated;

        private static void _RaiseParticipantUpdated(object[] args)
        {
            FireEvent(ParticipantUpdated, args);
        }

        internal static void AsyncRaiseParticipantUpdated(object sender, Multicast multicast, Participant participant, bool isLocal)
        {
            if (ParticipantUpdated == null) return;

            object[] args = { sender, new ParticipantEventArgs(multicast, participant,isLocal) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseParticipantUpdated), args);
        }

        #endregion

        #region 组播成员离开的事件

        public delegate void ParticipantRemovedEventHandler(object sender, ParticipantEventArgs ea);


        public static event ParticipantRemovedEventHandler ParticipantRemoved;

        private static void _RaiseParticipantRemoved(object[] args)
        {
            FireEvent(ParticipantRemoved, args);
        }

        internal static void AsyncRaiseParticipantRemoved(object sender, Multicast multicast, Participant participant, bool isLocal)
        {
            if (ParticipantRemoved == null) return;

            object[] args = { sender, new ParticipantEventArgs(multicast, participant, isLocal) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseParticipantRemoved), args);
        }

        #endregion

        #region 收到了rtp数据时触发

        public class MessageReceivedEventArgs : EventArgs
        {
            public Message Message
            {
                get;
                private set;
            }

            /// <summary>
            /// 消息发送的目的地址
            /// </summary>
            public string Destination
            {
                get;
                private set;
            }


            public MessageReceivedEventArgs(Message message)
            {
                this.Message = message;
                this.Destination = message.Header.GetValue<string>(MessageField.Destination, string.Empty);
            }
        }

        public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs ea);

        internal static event MessageReceivedEventHandler MessageReceived;

        private static void _RaiseMessageReceived(object[] args)
        {
            FireEvent(MessageReceived, args);
        }

        internal static void AsyncRaiseMessageReceived(object sender, Message msg)
        {
            if (MessageReceived == null) return;

            object[] args = { sender, new MessageReceivedEventArgs(msg) };
            AnycastEventThrower.QueueUserWorkItem(new RaiseEvent(_RaiseMessageReceived), args);
        }

        #endregion

        //#region 事件参数

        ///// <summary>
        ///// 组播参与人的事件参数
        ///// </summary>
        //public class RtpParticipantEventArgs : EventArgs
        //{
        //    public RtpParticipant RtpParticipant
        //    {
        //        get;
        //        private set;
        //    }

        //    public RtpMulticast Multicast
        //    {
        //        get;
        //        private set;
        //    }

        //    public RtpParticipantEventArgs(RtpMulticast multicast, RtpParticipant participant)
        //    {
        //        this.Multicast = multicast;
        //        this.RtpParticipant = participant;
        //    }
        //}

        //#endregion

        //#region 单播转发数据时的回调

        //public class RtpUnicastCallbackEventArgs : EventArgs
        //{
        //    public string UnicastAddress
        //    {
        //        get;
        //        private set;
        //    }


        //    public string EventSource
        //    {
        //        get;
        //        private set;
        //    }

        //    public bool Success
        //    {
        //        get;
        //        private set;
        //    }


        //    public RtpUnicastCallbackEventArgs(string unicastAddress, string eventSource, bool success)
        //    {
        //        this.UnicastAddress = unicastAddress;
        //        this.EventSource = eventSource;
        //        this.Success = success;
        //    }
        //}

        //public delegate void RtpUnicastCallbackEventHandler(object sender, RtpUnicastCallbackEventArgs ea);

        //public static event RtpUnicastCallbackEventHandler RtpUnicastCallback;

        //internal static void RaiseRtpUnicastCallbackEvent(object[] args)
        //{
        //    FireEvent(RtpUnicastCallback, args);
        //}

        //#endregion


        //#region 插件事件

        ///// <summary>
        ///// 收到插件相关的数据包
        ///// </summary>
        //public class RtpDataPackagePluginReceivedEventArgs : EventArgs
        //{
        //    public RtpDataPackage DataPackage
        //    {
        //        get;
        //        private set;
        //    }

        //    public string PluginName
        //    {
        //        get
        //        {
        //            return this.DataPackage.PluginName;
        //        }
        //    }

        //    public string EventSource
        //    {
        //        get
        //        {
        //            return this.DataPackage.EventSource;
        //        }
        //    }


        //    public RtpDataPackagePluginReceivedEventArgs(RtpDataPackage package)
        //    {
        //        this.DataPackage = package;
        //    }
        //}

        //public delegate void RtpDataPackagePluginReceivedEventHandler(object sender, RtpDataPackagePluginReceivedEventArgs ea);

        //public static event RtpDataPackagePluginReceivedEventHandler RtpDataPackagePluginReceived;

        //internal static void RaiseRtpDataPackagePluginReceivedEvent(object[] args)
        //{
        //    FireEvent(RtpDataPackagePluginReceived, args);
        //}


        //#endregion
    }
}
