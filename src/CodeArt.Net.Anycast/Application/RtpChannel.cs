using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent.Pattern;
using System.Collections.Concurrent;

namespace CodeArt.Net.Anycast
{
    public class RtpChannel : IDisposable
    {
        public RtpContext Context
        {
            get;
            private set;
        }

        /// <summary>
        /// 通道驻留的地址
        /// </summary>
        public string HostAddress
        {
            get;
            private set;
        }

        public Participant Participant
        {
            get;
            private set;
        }

        internal int ReferenceCount
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        internal RtpChannel(RtpContext context, string hostAddress, Participant participant)
        {
            this.Context = context;
            this.HostAddress = hostAddress;
            this.Participant = participant;
            this.ReferenceCount = 0;
        }

        #region 通道拥有的能力

        private List<RtpCapability> _capabilities = new List<RtpCapability>();

        /// <summary>
        /// 注册Capability,请保证每个Capability是线程安全的
        /// </summary>
        /// <param name="finder"></param>
        public void RegisterCapability(RtpCapability capability)
        {
            lock (_capabilities)
            {
                var exist = GetCapability(capability.Name) != null;
                if (!exist)
                {
                    capability.Join(this);
                    _capabilities.Add(capability);
                }
            }
        }

        private RtpCapability GetCapability(string capabilityName)
        {
            RtpCapability result = null;

            lock (_capabilities)
            {
                foreach (var capability in _capabilities)
                {
                    if (capability.Name == capabilityName)
                    {
                        result = capability;
                        break;
                    }
                }
            }
            return result;
        }

        public T GetCapability<T>() where T : RtpCapability
        {
            return GetCapability(typeof(T).Name) as T;
        }

        public T GetCapability<T>(string name) where T : RtpCapability
        {
            return GetCapability(name) as T;
        }

        /// <summary>
        /// 注销某项能力
        /// </summary>
        /// <param name="address"></param>
        public void LogoutCapability(string name)
        {
            lock (_capabilities)
            {
                var capability = GetCapability(name);
                LogoutCapability(capability);
            }
        }

        private void LogoutCapability(RtpCapability capability)
        {
            if (capability != null)
            {
                capability.Leave();
                _capabilities.Remove(capability);
                capability.Dispose();
            }
        }


        /// <summary>
        /// 注销所有能力
        /// </summary>
        public void LogoutCapabilities()
        {
            lock (_capabilities)
            {
                var result = _capabilities.ToArray();

                foreach (var capability in result)
                {
                    LogoutCapability(capability);
                }
            }
        }

        #endregion

        private void DisposeCapabilities()
        {
            lock (_capabilities)
            {
                foreach (var capability in _capabilities)
                {
                    capability.Dispose();
                }
                _capabilities.Clear();
            }
        }


        internal void Process(ClientEvents.MessageReceivedEventArgs ea)
        {
            RtpCapability.Process(this.Context.Client, ea); //新版本中处理消息，统一用能力事件
            //var capabilityName = RtpCapability.GetCapabilityName(ea.Message);
            //var capability = GetCapability(capabilityName);
            ////if (capability == null) throw new ApplicationException("在地址" + this.Address + "的rtp通道上没有给予名称为" + capabilityName + "的能力");
            //if (capability == null) return; //这代表虽然加入了该地址，但是并不需要这个能力，不予处理
            //capability.Process(ea);
        }

        public void Dispose()
        {
            this.ReferenceCount = 0;
            DisposeCapabilities();
        }

    }
}
