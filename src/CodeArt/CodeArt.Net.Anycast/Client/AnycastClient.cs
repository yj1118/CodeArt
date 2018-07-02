using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Threading;

using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Handlers.Timeout;

using CodeArt.DTO;
using CodeArt.Concurrent.Pattern;
using CodeArt.Concurrent;
using DotNetty.Buffers;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 任播客户端
    /// </summary>
    public class AnycastClient : IDisposable
    {
        public ClientConfig Config
        {
            get;
            private set;
        }

        public IPEndPoint ServerEndPoint
        {
            get;
            private set;
        }

        public IChannel Channel
        {
            get;
            internal set;
        }

        private IEventLoopGroup _group;

        internal ConnectionStatus Status
        {
            get;
            set;
        }

        public bool IsActive
        {
            get
            {
                return Channel != null && Channel.Active && this.Status == ConnectionStatus.Connected;
            }
        }

        /// <summary>
        /// 客户端所在的地址
        /// </summary>
        public string Address
        {
            get;
            private set;
        }

        /// <summary>
        /// 如果不是同一个线程创建的缓冲区，那么缓冲区是不能重用的，所以
        /// </summary>
        private ActionPipeline _pipeline;

        public AnycastClient(ClientConfig config)
        {
            this.Config = config;
            this.Status = ConnectionStatus.Disconnected;

            _listeners = new List<IMessageListener>();

            _pipeline = new ActionPipeline();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tryReconnect">当连接失败后，是否重连（该设置不会影响掉线重连的情况）</param>
        /// <returns></returns>
        public void Connect(bool tryReconnect = false)
        {
            _pipeline.Queue(() =>
            {
                if (tryReconnect)
                {
                    _Connect(new ReconnectArgs());
                }
                else
                {
                    _Connect(null);
                }
            });
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        private void _Connect(ReconnectArgs reconnectArgs)
        {
            if (_disposed) return;
            if (this.Status != ConnectionStatus.Disconnected) return;
            this.Status = ConnectionStatus.Connecting;

            if(_group == null)
                _group = new MultithreadEventLoopGroup();

            X509Certificate2 cert = null;
            string targetHost = null;
            if (this.Config.IsSsl)
            {
                cert = new X509Certificate2(Path.Combine(AppContext.ProcessDirectory, "anycast.pfx"), "password");
                targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            }

            var bootstrap = new Bootstrap();
            bootstrap
                .Group(_group)
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.Allocator, UnpooledByteBufferAllocator.Default) //由于断线重连有内存泄露问题，所以我们使用 非池的字节缓冲区
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;

                    if (cert != null)
                    {
                        pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                    }
                    pipeline.AddLast(new LoggingHandler());
                    pipeline.AddLast("framing-enc", new MessageEncoder());
                    pipeline.AddLast("framing-dec", new MessageDecoder());

                    pipeline.AddLast("ReadTimeout", new ReadTimeoutHandler(Settings.Timeout)); //超过指定时间没有接收到任何信息，自动关闭
                    pipeline.AddLast("LoginAuthReq", new LoginAuthReqHandler(this));
                    pipeline.AddLast("HeartBeatReq", new HeartBeatReqHandler(this));
                    pipeline.AddLast("ClientLogic", new ClientLogicHandler(this));
                }));

            this.ServerEndPoint = new IPEndPoint(IPAddress.Parse(this.Config.Host), this.Config.Port);
            ClientEvents.AsyncRaiseConnecting(this, reconnectArgs);

            try
            {
                if (reconnectArgs != null && reconnectArgs.Times > 0)  //第0次重连意味着是直接连接，不用等待
                {
                    _reconnectCancellation = new CancellationTokenSource();
                    Task.Delay(TimeSpan.FromSeconds(this.Config.ReconnectDelayTime), _reconnectCancellation.Token).Wait();
                }

                this.Channel = bootstrap.ConnectAsync(this.ServerEndPoint).Result;
                this.Address = this.Channel.LocalAddress.ToString();  //在client，我们的地址是通道的本地地址

                //while (true)
                //{
                //    this.Channel = bootstrap.ConnectAsync(this.ServerEndPoint).Result;
                //    this.Address = this.Channel.LocalAddress.ToString();  //在client，我们的地址是通道的本地地址
                //    System.Threading.Thread.Sleep(2000);
                //    this.Channel.CloseAsync().Wait();
                //}


            }
            catch (Exception ex)
            {
                this.Status = ConnectionStatus.Disconnected;
                var args = reconnectArgs?.Clone();
                ClientEvents.AsyncRaiseError(this, new ConnectServerException(this.ServerEndPoint, ex, args));
                if (reconnectArgs != null)
                {
                    _pipeline.Queue(()=>
                    {
                        _Reconnect(reconnectArgs);
                    });
                }
            }
        }

        /// <summary>
        /// 客户端已掉线
        /// </summary>
        internal void Dropped()
        {
            _pipeline.Queue(() =>
            {
                _Disconnect(true);
            });
        }

        private void _Disconnect(bool isDropped)
        {
            if (_disposed) return;
            if (this.Status != ConnectionStatus.Connected) return;
            this.Status = ConnectionStatus.Disconnected;

            try
            {
                DisposeReconnectCancellation();

                if (Channel != null)
                {
                    this.LeaveAll();
                    _DisposeChannel();
                    ClientEvents.AsyncRaiseDisconnected(this, isDropped);
                }
            }
            catch(Exception ex)
            {
                ClientEvents.AsyncRaiseError(this, ex);
            }
            finally
            {
                if (isDropped)
                {
                    _pipeline.Queue(() =>
                    {
                        _Reconnect(new ReconnectArgs());
                    });
                }
            }
        }

        private CancellationTokenSource _reconnectCancellation;

        private void DisposeReconnectCancellation()
        {
            if (_reconnectCancellation != null)
            {
                _reconnectCancellation.Cancel(true);
                _reconnectCancellation.Dispose();
                _reconnectCancellation = null;
            }
        }

        /// <summary>
        /// 重连
        /// </summary>
        private void _Reconnect(ReconnectArgs arg)
        {
            DisposeReconnectCancellation();

            if (this.Config.ReconnectTimes > 0 && arg.Times == this.Config.ReconnectTimes)
            {
                //this.Config.ReconnectTimes小于或者等于0的时候，不触发错误，因为无限重连
                ClientEvents.AsyncRaiseError(this, new ReconnectFailedException());
                return;
            }

            arg.Times++;
            _Connect(arg);
        }

        private void _DisposeChannel()
        {
            if (this.Channel != null)
            {
                this.Channel.DisconnectAsync().Wait();
                this.Channel.CloseAsync().Wait();
                this.Address = null;
                this.Channel = null;
            }
        }

        private bool _disposed = false;

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _pipeline.Queue(() =>
            {
                if (_disposed) return;
                _Disconnect(false);
                _DisposeChannel(); //防止连接状态导致通道还是未被释放，所以这里再次释放一次
                if (_group != null)
                {
                    _group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)).Wait();
                    _group = null;
                }
                _disposed = true;
                _pipeline.Dispose();
            });
        }


        #region 消息处理

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="message"></param>
        public void Process(Message message)
        {
            foreach (var listener in _listeners)
                listener.Process(this, message);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        public Future<bool> Send(Message message)
        {
            var future = new Future<bool>();
            future.Start();
            _pipeline.Queue(() =>
            {
                if (this.Channel == null)
                    future.SetResult(false);
                else
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            var task = Channel?.WriteAndFlushAsync(message);
                            task.Wait();
                            future.SetResult(true);
                        }
                        catch (Exception ex)
                        {
                            future.SetError(ex);
                        }
                    });
                }
            });
            return future;
        }

        #endregion

        private List<IMessageListener> _listeners;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public AnycastClient AddListener(IMessageListener listener)
        {
            _listeners.Add(listener);
            return this;
        }


        #region 组播地址

        private List<Multicast> _multicasts = new List<Multicast>();

        /// <summary>
        /// 由于并发性的原因，我们仅提供Use方法，而不提供Get方法直接返回组播对象
        /// </summary>
        /// <param name="multicastAddress"></param>
        /// <param name="action"></param>
        public void UseMulticast(string multicastAddress,Action<Multicast> action)
        {
            lock (_multicasts)
            {
                var multicast = GetMulticast(multicastAddress);
                if (multicast == null) return;
                action(multicast);
            }
        }


        private Multicast GetMulticast(string multicastAddress)
        {
            lock (_multicasts)
            {
                return _multicasts.FirstOrDefault((t) =>
                {
                    return t.Address == multicastAddress;
                });
            }
        }


        /// <summary>
        /// 该回话参与到的组播地址中
        /// </summary>
        public Multicast[] Multicasts
        {
            get
            {
                lock (_multicasts)
                {
                    return _multicasts.ToArray();
                }
            }
        }

        public Multicast Join(string multicastAddress, Participant participant)
        {
            var multicast = GetMulticast(multicastAddress);
            if (multicast != null) return multicast;

            lock (_multicasts)
            {
                multicast = GetMulticast(multicastAddress);
                if (multicast != null) return multicast;

                multicast = new Multicast(this, multicastAddress, participant);
                multicast.Join();
                _multicasts.Add(multicast);
            }
            return multicast;
        }

        public void Leave(string multicastAddress)
        {
            var multicast = GetMulticast(multicastAddress);
            if (multicast == null) return;

            lock (_multicasts)
            {
                multicast = GetMulticast(multicastAddress);
                if (multicast == null) return;

                multicast.Leave();
                _multicasts.Remove(multicast);
            }
        }

        /// <summary>
        /// 离开所有组播
        /// </summary>
        private void LeaveAll()
        {
            var multicasts = this.Multicasts;
            foreach (var multicast in multicasts)
            {
                Leave(multicast.Address);
            }
        }

        #endregion

        #region  参与者

        public Participant[] GetParticipants(string multicastAddress)
        {
            var multicast = GetMulticast(multicastAddress);
            return multicast == null ? Array.Empty<Participant>() : multicast.Participants;
        }

        public Participant GetParticipant(string multicastAddress, string participantId)
        {
            var multicast = GetMulticast(multicastAddress);
            return multicast == null ? null : multicast.GetParticipant(participantId);
        }

        public void UpdateParticipant(string multicastAddress, Participant participant)
        {
            var multicast = GetMulticast(multicastAddress);
            if (multicast == null) return;

            lock (_multicasts)
            {
                multicast = GetMulticast(multicastAddress);
                if (multicast == null) return;
                multicast.UpdateHost(participant);
            }
        }


        #endregion



    }
}