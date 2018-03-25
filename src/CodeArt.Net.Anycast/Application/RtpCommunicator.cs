using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CodeArt.Net.Anycast.AnycastEventsBase;

using CodeArt.Event;
using CodeArt.Util;

namespace CodeArt.Net.Anycast
{
    public class RtpCommunicator : IDisposable
    {
        private RtpContext _context;

        public RtpContext Context
        {
            get
            {
                return _context;
            }
        }

        public bool IsConnected
        {
            get
            {
                return  _context.IsConnected;
            }
        }

        #region 事件

        /// <summary>
        /// 第一次出现连接错误时触发（连接成功后，掉线了，再连接时，第一次出现的错误也会触发）
        /// </summary>
        public event ErrorEventHandler FirstConnectError;

        /// <summary>
        /// 最后一次出现连接错误时触发，这意味着多次尝试重连后还是无法连接服务器
        /// </summary>
        public event ErrorEventHandler LastConnectError;

        public event ErrorEventHandler Error;

        public event EventHandler Connected;

        public event EventHandler Dropped;

        #endregion

        #region 成员事件挂载 

        private void OnParticipantAdded(object sender, ClientEvents.ParticipantEventArgs ea)
        {
            if (sender != this.Context.Client) return;

            RaiseParticipantChanged(ea.Multicast, ea.Participant, ParticipantChangedType.Added);
        }

        private void OnParticipantUpdated(object sender, ClientEvents.ParticipantEventArgs ea)
        {
            if (sender != this.Context.Client) return;

            RaiseParticipantChanged(ea.Multicast, ea.Participant, ParticipantChangedType.Updated);
        }


        private void OnParticipantRemoved(object sender, ClientEvents.ParticipantEventArgs ea)
        {
            if (sender != this.Context.Client) return;

            RaiseParticipantChanged(ea.Multicast, ea.Participant, ParticipantChangedType.Removed);

            if (ea.IsLocal)
            {
                //这表示本地负责人退出了组播，我们需要移除所有与该模块有关的参数
                _participantTrigger.UnRaise((arg) =>
                {
                    var p = arg as ParticipantChangedEventArgs;
                    if (p == null) return false;
                    return p.Multicast == ea.Multicast;
                });
            }
        }

        private void RaiseParticipantChanged(Multicast multicast, Participant participant, ParticipantChangedType type)
        {
            _participantTrigger.Raise(new ParticipantChangedEventArgs(this, multicast, participant, type));
        }


        //-------------静态公开方法 start ----------------

        private static readonly ValidEventTrigger _participantTrigger = new ValidEventTrigger();

        public static void HookParticipantChanged(ValidEventHandler handle)
        {
            _participantTrigger.Hook(handle);
        }

        public static void UnhookParticipantChanged(ValidEventHandler handle)
        {
            _participantTrigger.Unhook(handle);
        }

        //-------------静态公开方法 end ----------------


        #endregion

        /// <summary>
        /// 通讯器的唯一名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        public RtpCommunicator(string name, ClientConfig config)
        {
            this.Name = name;
            _modulesIndex = new MultiDictionary<IRtpModuleFactory, RtpModule>(true);
            _moduleFactories = new List<IRtpModuleFactory>();
            _context = new RtpContext(config, Participant.Empty);
            _context.Active += OnContextActive;
            _context.Inactive += OnContextInactive;
            _context.ParticipantAdded += OnParticipantAdded;
            _context.ParticipantUpdated += OnParticipantUpdated;
            _context.ParticipantRemoved += OnParticipantRemoved;
        }


        private void OnContextActive(object sender, EventArgs e)
        {
            lock(_syncModule)
            {
                foreach (var factory in _moduleFactories)
                {
                    InstallModule(factory);
                }
            }
        }

        private void OnContextInactive(object sender, EventArgs e)
        {
            lock (_syncModule)
            {
                foreach (var factory in _moduleFactories)
                {
                    UninstallModule(factory);
                }
            }
        }

        #region 模块管理

        private object _syncModule = new object();

        private MultiDictionary<IRtpModuleFactory, RtpModule> _modulesIndex;

        private List<IRtpModuleFactory> _moduleFactories;

        /// <summary>
        /// 为通讯器添加模块，当通讯器活动时，该模块会被安装
        /// </summary>
        /// <param name="module"></param>
        public void AddModule(IRtpModuleFactory factory)
        {
            lock(_syncModule)
            {
                _moduleFactories.Add(factory);
                if(_context.IsConnected)
                {
                    InstallModule(factory);
                }
            }
        }

        /// <summary>
        /// 安装模块
        /// </summary>
        /// <param name="factory"></param>
        private void InstallModule(IRtpModuleFactory factory)
        {
            if (_modulesIndex.ContainsKey(factory)) return; //模块已安装，不重复安装

            var modules = factory.Create();
            if (modules == null) return; //有可能工厂中的算法得到的是空的模块，这种模块不加载
            foreach (var module in modules)
            {
                _modulesIndex.Add(factory, module);
                module.Install(this);
            }
        }

        /// <summary>
        /// 移除模块
        /// </summary>
        /// <param name="module"></param>
        public void RemoveModule(IRtpModuleFactory factory)
        {
            lock (_syncModule)
            {
                if (_moduleFactories.Remove(factory))
                {
                    UninstallModule(factory);
                }
            }
        }

        /// <summary>
        /// 卸载模块
        /// </summary>
        /// <param name="factory"></param>
        private void UninstallModule(IRtpModuleFactory factory)
        {
            if (_modulesIndex.TryGetValue(factory, out var modules))
            {
                foreach(var module in modules)
                {
                    module.Uninstall(this);
                }
                _modulesIndex.Remove(factory);
            }
        }



        #endregion


        public void Start()
        {
            InitializeEvents();

            _context.Connect(true);
        }

        private void InitializeEvents()
        {
            ClientEvents.Connecting += OnConnecting;
            ClientEvents.Error += OnError;
            ClientEvents.Connected += OnConnected;
            ClientEvents.Disconnected += OnDisconnected;
        }

        private void DisposeEvents()
        {
            ClientEvents.Connecting -= OnConnecting;
            ClientEvents.Error -= OnError;
            ClientEvents.Connected -= OnConnected;
            ClientEvents.Disconnected -= OnDisconnected;
        }

        /// <summary>
        /// 停止通讯，但是装载的模块会被保留，下次启动时，会自动加载模块
        /// </summary>
        public void Stop()
        {
            DisposeEvents();
            _context.Disconnect();
            _firstConnectServer = true; //重置相关状态
        }

        public void Dispose()
        {
            this.Stop();
            _context.Dispose();
        }

        public override bool Equals(object obj)
        {
            var target = obj as RtpCommunicator;
            if (target == null) return false;
            return target.Name == this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        public void Reset()
        {
            Stop();
            Start();
        }

        /// <summary>
        /// 是否为第一次连接服务器
        /// </summary>
        private bool _firstConnectServer = true;

        private void OnError(object sender, AnycastEventsBase.ErrorEventArgs ea)
        {
            if (_context.Client != sender) return;

            if (ea.Exception is ConnectServerException)
            {
                if (_firstConnectServer)
                {
                    _firstConnectServer = false;
                    if (this.FirstConnectError != null)
                        this.FirstConnectError(this, ea);
                }
                return;
            }

            if(ea.Exception is ReconnectFailedException)
            {
                if (this.LastConnectError != null)
                    this.LastConnectError(this, ea);
                return;
            }

            if (this.Error != null)
                this.Error(this, ea);
        }

        private static void OnConnecting(object sender, ClientEvents.ConnectingEventArgs ea)
        {
            //将连接的信息隐藏在幕后
        }

        private void OnConnected(object sender, ClientEvents.ConnectedEventArgs ea)
        {
            if (_context.Client != sender) return;
            _firstConnectServer = false; //连接成功后，就重置状态

            if (this.Connected != null)
                this.Connected(sender, ea);
        }

        private void OnDisconnected(object sender, ClientEvents.DisconnectedEventArgs ea)
        {
            if (_context.Client != sender) return;

            if (ea.IsDropped)
            {
                if (this.Dropped != null)
                {
                    this.Dropped(sender, ea);
                }
                return;
            }
        }
    }
}
