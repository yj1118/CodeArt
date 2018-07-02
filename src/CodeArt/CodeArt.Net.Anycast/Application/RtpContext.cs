using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 每个环境对象连接后，会默认产生一个单播通道
    /// 每个环境对象可以开通多个组播通道
    /// 每个通道都可以注册多种能力
    /// 每种能力对应不同的业务需要
    /// </summary>
    public class RtpContext : IDisposable
    {
        public AnycastClient Client
        {
            get;
            private set;
        }

        public bool IsConnected
        {
            get
            {
                return this.Client != null && this.Client.IsActive;
            }
        }

        public RtpChannel UnicastChannel
        {
            get;
            private set;
        }

        /// <summary>
        /// 默认的负责人，该负责人会驻留单播通道
        /// </summary>
        public Participant DefaultParticipant
        {
            get;
            private set;
        }

        private ClientConfig _config;

        public RtpContext(ClientConfig config, Participant defaultParticipant)
        {
            _config = config;
            this.DefaultParticipant = defaultParticipant;
            HookFixedEvents();
        }

        #region 固化的事件

        /// <summary>
        /// 在对象生存期内，连接成功和断开连接的事件是必须挂载的
        /// 再由这两个事件来处理其他的逻辑
        /// </summary>
        private void HookFixedEvents()
        {
            ClientEvents.Connected += OnConnected;
            ClientEvents.Disconnected += OnDisconnected;
        }

        private void UnhookFixedEvents()
        {
            ClientEvents.Connected -= OnConnected;
            ClientEvents.Disconnected -= OnDisconnected;
        }

        /// <summary>
        /// 成功连接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void OnConnected(object sender, ClientEvents.ConnectedEventArgs ea)
        {
            if (sender != this.Client) return;

            InitUnicastChannel();
            HookRtpEvents();

            if (this.Active != null) this.Active(this, EventArgs.Empty);
        }

        /// <summary>
        /// 断开连接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void OnDisconnected(object sender, ClientEvents.DisconnectedEventArgs ea)
        {
            if (sender != this.Client) return;

            UnhookRtpEvents();
            DisposeUnicastChannel();
            DisposeChannels();

            if(_disposed)
            {
                UnhookFixedEvents();
            }

            if (this.Inactive != null) this.Inactive(this, EventArgs.Empty);
        }

        #endregion

        public void Connect(bool tryReconnect = false)
        {
            if (this.IsConnected) return;
            this.Client = new AnycastClient(_config);
            this.Client.Connect(tryReconnect);
        }

        public void Disconnect()
        {
            if (this.Client != null)
            {
                this.Client.Dispose();
            }
        }

        private void ValidateConnected()
        {
            if (!this.IsConnected) throw new InvalidOperationException(Strings.NotConnectedServer);
        }

        #region 有效和无效事件

        /// <summary>
        /// 当对象处于活动状态时触发该事件
        /// </summary>
        public event EventHandler Active;

        /// <summary>
        /// 当对象处于非活动状态时触发该事件
        /// </summary>
        public event EventHandler Inactive;

        #endregion


        #region 成员

        public Participant GetParticipant(string multicastAddress, string participantId)
        {
            ValidateConnected();
            return this.Client.GetParticipant(multicastAddress, participantId);
        }

        public Participant[] GetParticipants(string multicastAddress)
        {
            ValidateConnected();
            return this.Client.GetParticipants(multicastAddress);
        }


        public event ClientEvents.ParticipantAddedEventHandler ParticipantAdded;

        private void OnParticipantAdded(object sender, ClientEvents.ParticipantEventArgs ea)
        {
            if (sender != this.Client) return;

            if (this.ParticipantAdded != null)
                this.ParticipantAdded(sender, ea);
        }


        public event ClientEvents.ParticipantAddedEventHandler ParticipantUpdated;

        private void OnParticipantUpdated(object sender, ClientEvents.ParticipantEventArgs ea)
        {
            if (sender != this.Client) return;

            if (this.ParticipantUpdated != null)
                this.ParticipantUpdated(sender, ea);
        }


        /// <summary>
        /// 当检测一个参与者离开时触发。
        /// </summary>
        public event ClientEvents.ParticipantRemovedEventHandler ParticipantRemoved;

        private void OnParticipantRemoved(object sender, ClientEvents.ParticipantEventArgs ea)
        {
            if (sender != this.Client) return;

            if (ParticipantRemoved != null)
            {
                ParticipantRemoved(sender, ea);
            }
        }

        #endregion

        #region 事件处理

        private void HookRtpEvents()
        {
            ClientEvents.ParticipantAdded += OnParticipantAdded;
            ClientEvents.ParticipantRemoved += OnParticipantRemoved;
            ClientEvents.ParticipantUpdated += OnParticipantUpdated;
            ClientEvents.MessageReceived += OnMessageReceived;
            //ClientEvents.RtpUnicastCallback += OnRtpUnicastCallback;
        }

        private void UnhookRtpEvents()
        {
            ClientEvents.ParticipantAdded -= OnParticipantAdded;
            ClientEvents.ParticipantRemoved -= OnParticipantRemoved;
            ClientEvents.ParticipantUpdated -= OnParticipantUpdated;
            ClientEvents.MessageReceived -= OnMessageReceived;
            //ClientEvents.RtpUnicastCallback -= OnRtpUnicastCallback;
        }

        #endregion

        #region 单播数据通道

        /// <summary>
        /// 初始化单播通道
        /// </summary>
        /// <returns></returns>
        private void InitUnicastChannel()
        {
            ValidateConnected();
            this.UnicastChannel = new RtpChannel(this, this.Client.Address, this.DefaultParticipant);
        }

        private void DisposeUnicastChannel()
        {
            this.UnicastChannel.Dispose();
            this.UnicastChannel = null;
        }

        #endregion

        #region 多播数据通道

        private List<RtpChannel> _multicastChannels = new List<RtpChannel>();

        /// <summary>
        /// 创建或得到一个基于多播地址的通道，如果通道已存在，那么返回当前通道，并追加引用计数
        /// </summary>
        /// <param name="multicastAddress"></param>
        /// <returns></returns>
        public RtpChannel CreateChannel(string multicastAddress, Participant participant)
        {
            ValidateConnected();
            lock (_multicastChannels)
            {
                var channel = GetMulticastChannel(multicastAddress);
                if (channel == null)
                {
                    channel = new RtpChannel(this, multicastAddress, participant);
                    _multicastChannels.Add(channel);
                    this.Client.Join(multicastAddress, participant);
                }
                channel.ReferenceCount++;
                return channel;
            }
            //throw new InvalidOperationException(string.Format(Strings.ChannelExistsOnAddress, multicastAddress));
        }

        public Participant UpdateParticipant(string multicastAddress, Action<Participant> update)
        {
            ValidateConnected();
            lock (_multicastChannels)
            {
                var channel = GetMulticastChannel(multicastAddress);
                if (channel != null)
                {
                    update(channel.Participant);
                    this.Client.UpdateParticipant(multicastAddress, channel.Participant);
                    return channel.Participant;
                }
            }
            return null;
        }


        public RtpChannel GetMulticastChannel(string multicastAddress)
        {
            RtpChannel result = null;

            lock (_multicastChannels)
            {
                foreach (var channel in _multicastChannels)
                {
                    if (channel.HostAddress == multicastAddress)
                    {
                        result = channel;
                        break;
                    }
                }
            }
            return result;
        }

        public void RemoveChannel(string multicastAddress)
        {
            lock (_multicastChannels)
            {
                var channel = GetMulticastChannel(multicastAddress);
                if (channel != null)
                {
                    channel.ReferenceCount--;
                    if (channel.ReferenceCount == 0)
                    {
                        this.Client.Leave(multicastAddress);
                        _multicastChannels.Remove(channel);
                        channel.Dispose();
                    }
                }
            }
        }

        private void DisposeChannels()
        {
            lock (_multicastChannels)
            {
                foreach (var channel in _multicastChannels)
                {
                    this.Client.Leave(channel.HostAddress);
                    channel.Dispose();
                }
                _multicastChannels.Clear();
            }
        }

        private void OnMessageReceived(object sender, ClientEvents.MessageReceivedEventArgs ea)
        {
            if (sender != this.Client) return;

            //获取新的数据，交由通道处理
            var adderss = ea.Destination;
            if (string.IsNullOrEmpty(adderss)) return;

            if (adderss == this.UnicastChannel.HostAddress)
            {
                this.UnicastChannel.Process(ea);
            }
            else
            {
                var channel = GetMulticastChannel(adderss);
                if (channel == null) throw new ApplicationException(string.Format(Strings.NoChannelCreatedAtAddress, adderss));
                channel.Process(ea);
            }
        }

        #endregion

        #region 能力

        /// <summary>
        /// 该方法很重要，会在上下文中找到匹配的能力
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCapability<T>() where T : RtpCapability
        {
            var capability = this.UnicastChannel.GetCapability<T>();
            if (capability != null) return capability;
            lock (_multicastChannels)
            {
                foreach (var channel in _multicastChannels)
                {
                    capability = channel.GetCapability<T>();
                    if (capability != null) return capability;
                }
            }
            throw new InvalidOperationException(string.Format(Strings.NoCapabilityAtContext, typeof(T).Name));
        }

        public T GetCapability<T>(RtpChannel channel) where T : RtpCapability
        {
            var capability = this.UnicastChannel.GetCapability<T>();
            if (capability != null) return capability;
            lock (_multicastChannels)
            {
                foreach (var mychannel in _multicastChannels)
                {
                    if (mychannel == channel)
                        capability = channel.GetCapability<T>();

                    if (capability != null) return capability;
                }
            }
            throw new InvalidOperationException(string.Format(Strings.NoCapabilityAtContext, typeof(T).Name));
        }

        #endregion

        #region 销毁

        private bool _disposed = false;

        public void Dispose()
        {
            _disposed = true;
            if(this.IsConnected)
            {
                Disconnect();
            }
            else
            {
                UnhookFixedEvents();
            }
        }

        #endregion

    }
}
