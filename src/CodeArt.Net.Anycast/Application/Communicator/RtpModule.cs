using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Net.Anycast;

namespace CodeArt.Net.Anycast
{
    public abstract class RtpModule : IRtpModule
    {
        /// <summary>
        /// 模块是否已安装
        /// </summary>
        public bool Installed
        {
            get;
            private set;
        }

        public RtpCommunicator Communicator
        {
            get;
            private set;
        }

        /// <summary>
        /// 模块的负责人
        /// </summary>
        public Participant Participant
        {
            get;
            private set;
        }

        public RtpModule(Participant participant)
        {
            this.Participant = participant;
        }


        #region 通道

        private RtpChannel _channel;

        protected abstract RtpChannel CreateChannel(RtpCommunicator communicator);

        protected abstract void RemoveChannel(RtpCommunicator communicator);

        /// <summary>
        /// 更改并传递负责人信息
        /// </summary>
        /// <param name="action"></param>
        public void UpdateParticipant(Action<Participant> action)
        {
            this.Participant = UpdateParticipantImpl(action);
        }

        protected abstract Participant UpdateParticipantImpl(Action<Participant> action);

        #endregion

        #region 能力

        private List<RtpCapability> _capabilities = new List<RtpCapability>();

        /// <summary>
        /// 用该方法注册能力，会自动卸载
        /// </summary>
        /// <param name="capability"></param>
        protected void RegisterCapability(RtpCapability capability)
        {
            if (_channel != null)
            {
                _channel.RegisterCapability(capability); //切换会议的能力
                _capabilities.Add(capability);
            }
        }

        private void LogoutCapabilities()
        {
            if (_channel != null)
            {
                foreach (var capability in _capabilities)
                {
                    _channel.LogoutCapability(capability.Name); //注销能力
                }
            }
        }

        public T GetCapability<T>() where T : RtpCapability
        {
            return (T)GetCapability((t) =>
            {
                return t is T;
            });
        }

        public RtpCapability GetCapability(Func<RtpCapability, bool> find)
        {
            var item = _capabilities.FirstOrDefault((t) =>
            {
                return find(t);
            });

            return item;
        }

        public static bool Use<MT,CT>(Action<MT,CT> action)
            where MT : RtpModule
            where CT : RtpCapability
        {
            return Use<MT>((module) =>
            {
                var capability = module.GetCapability<CT>();
                action(module, capability);
            });
        }

        #endregion

        /// <summary>
        /// 安装模块
        /// </summary>
        /// <param name="context"></param>
        public void Install(RtpCommunicator communicator)
        {
            if (this.Installed) return;
            this.Installed = true;
            this.Communicator = communicator;
            _channel = CreateChannel(communicator);
            InstallImpl(communicator);
            _events.Raise(communicator, this, true);
        }

        /// <summary>
        /// 安装模块的实现方法
        /// </summary>
        /// <param name="context"></param>
        protected abstract void InstallImpl(RtpCommunicator communicator);


        /// <summary>
        /// 移除模块
        /// </summary>
        /// <param name="context"></param>
        public void Uninstall(RtpCommunicator communicator)
        {
            if (!this.Installed) return;
            this.Installed = false;
            UninstallImpl(communicator);

            LogoutCapabilities();
            RemoveChannel(communicator);
            _channel = null;
            this.Communicator = null;
            _events.Raise(communicator, this, false);
        }

        /// <summary>
        /// 卸载模块的实现方法
        /// </summary>
        /// <param name="context"></param>
        protected abstract void UninstallImpl(RtpCommunicator communicator);

        #region 模块被安装/卸载/使用

        private static ModuleEventTrigger _events = new ModuleEventTrigger();

        /// <summary>
        /// 挂载模块被安装的事件
        /// </summary>
        /// <param name="handle"></param>
        public static void HookModuleChanged(ModuleChangedEventHandler handle)
        {
            _events.Hook(handle);
        }

        public static void UnhookModuleChanged(ModuleChangedEventHandler handle)
        {
            _events.Unhook(handle);
        }


        public static bool IsInstall(Func<RtpModule, bool> find)
        {
            return _events.IsInstall(find);
        }


        public static bool IsInstall<T>() where T : RtpModule
        {
            return _events.IsInstall((module)=>
            {
                return module is T;
            });
        }

        /// <summary>
        /// 判断模块是否属于某个组播地址
        /// </summary>
        /// <param name="find"></param>
        /// <param name="multicastAddress"></param>
        /// <returns></returns>
        public static bool IsSameAddress(Func<RtpModule, bool> find, string multicastAddress)
        {
            bool result = false;
            var success = Use(find, (module) =>
            {
                var multicastModule = module as RtpMulticastModule;
                result = (multicastModule != null && multicastModule.MulticastAddress == multicastAddress);
            });
            return result && success;
        }

        public static bool IsSameAddress<T>(string multicastAddress) where T : RtpModule
        {
            bool result = false;
            var success = Use<T>((module) =>
            {
                var multicastModule = module as RtpMulticastModule;
                result = (multicastModule != null && multicastModule.MulticastAddress == multicastAddress);
            });
            return result && success;
        }


        /// <summary>
        /// 使用模块，模块在使用时有可能会因为掉线等网络情况发生不可预知的异常，使用该方法可以避免频繁捕获错误
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns>成功使用module返回true，否则返回false</returns>
        public static bool Use<T>(Action<T> action) where T : RtpModule
        {
            return Use((module) =>
            {
                return module is T;
            }, (module) =>
            {
                action((T)module);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="find"></param>
        /// <param name="action"></param>
        /// <returns>成功使用module返回true，否则返回false</returns>
        public static bool Use(Func<RtpModule, bool> find, Action<RtpModule> action)
        {
            var module = _events.Get(find);
            if (module == null) return false;
            try
            {
                action(module);
            }
            catch (Exception ex)
            {
                //由于网络异步原因，使用module可能会出错，这里我们不抛出这类异常
                return false;
            }
            return true;
        }

        #endregion
    }
}