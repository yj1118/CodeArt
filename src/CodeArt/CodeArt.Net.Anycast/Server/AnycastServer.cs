using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Handlers.Timeout;
using DotNetty.Buffers;

namespace CodeArt.Net.Anycast
{
    /// <summary>
    /// 任播服务器
    /// </summary>
    public class AnycastServer : IDisposable
    {
        public ServerConfig Config
        {
            get;
            private set;
        }

        private IChannel _boundChannel;
        private IEventLoopGroup _bossGroup;
        private IEventLoopGroup _workerGroup;
        private bool _running;

        private List<IMessageHandler> _handlers;

        internal SessionManager Sessions
        {
            get;
            private set;
        }

        public AnycastServer(ServerConfig config)
        {
            this.Config = config;
            this.Sessions = new SessionManager(this);
            _handlers = new List<IMessageHandler>();
            _running = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public AnycastServer AddHandler(IMessageHandler handler)
        {
            _handlers.Add(handler);
            return this;
        }


        /// <summary>
        /// 运行
        /// </summary>
        public async Task Run()
        {
            if (_running) return;
            _running = true;

            _bossGroup = new MultithreadEventLoopGroup(1);
            _workerGroup = new MultithreadEventLoopGroup();

            X509Certificate2 tlsCertificate = null;
            if (this.Config.IsSsl)
            {
                tlsCertificate = new X509Certificate2(Path.Combine(AppContext.ProcessDirectory, "anycast.pfx"), "password");
            }

            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(_bossGroup, _workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 1024)
                .ChildOption<bool>(ChannelOption.SoKeepalive, true)
                .Handler(new LoggingHandler("AS-LSTN"))
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    channel.Configuration.Allocator = UnpooledByteBufferAllocator.Default; //由于PooledByteBuffer有内存不释放，所以我们使用 非池的字节缓冲区

                    IChannelPipeline pipeline = channel.Pipeline;
                    if (tlsCertificate != null)
                    {
                        pipeline.AddLast("tls", TlsHandler.Server(tlsCertificate));
                    }
                    pipeline.AddLast("framing-enc", new MessageEncoder());
                    pipeline.AddLast("framing-dec", new MessageDecoder());

                    pipeline.AddLast("ReadTimeout", new ReadTimeoutHandler(Settings.Timeout)); //超过指定时间没有接收到客户端任何信息，自动关闭对应的客户端通道
                    pipeline.AddLast("LoginAuthResp", new LoginAuthRespHandler(this));
                    pipeline.AddLast("HeartBeatResp", new HeartBeatRespHandler(this));
                    pipeline.AddLast("ServerLogic", new ServerLogicHandler(this));
                }));

            try
            {
                _boundChannel = await bootstrap.BindAsync(this.Config.Port);
                ServerEvents.AsyncRaiseRunning(this, this);
            }
            catch(Exception ex)
            {
                _running = false;
                ServerEvents.AsyncRaiseError(this, new RunServerException(ex));
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public async Task Stop()
        {
            if (!_running) return;
            _running = false;

            try
            {
                await _boundChannel.CloseAsync();
            }
            finally
            {
                await Task.WhenAll(
                    _bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    _workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }

        public void Dispose()
        {
            Task.WhenAll(this.Stop());
        }

        #region 消息处理

        internal void StartProcess(AnycastServer server, IServerSession origin, Message message, HandlerContext ctx)
        {
            foreach (var handler in _handlers)
            {
                handler.BeginProcess(server, origin, message, ctx);
            }
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="message"></param>
        internal void EndProcess(AnycastServer server, IServerSession origin, Message message, HandlerContext ctx)
        {
            foreach (var handler in _handlers)
            {
                handler.EndProcess(server, origin, message, ctx);
            }
        }

        #endregion

        public void Send(Message msg, string address)
        {
            var sessions = this.Sessions.GetAnySessions(address);

            Parallel.ForEach(sessions, (target) =>
            {
                if (!target.IsActive) return;

                var msgCopy = msg.Clone();
                msgCopy.Header.SetValue(MessageField.Destination, address); //设置目标地址
                msgCopy.Header.SetValue(MessageField.Origin, "server"); //设置来源地址，这里就是server
                target.Process(msgCopy);
            });

        }
    }
}