using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    public class ClientConfig
    {
        public bool IsSsl
        {
            get;
            set;
        }

        public string Host
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public IIdentityProvider IdentityProvider
        {
            get;
            set;
        }

        /// <summary>
        /// 尝试重连服务器的次数，该值小于等于0时表示无限次数重连
        /// </summary>
        public int ReconnectTimes
        {
            get;
            set;
        }

        /// <summary>
        /// 重连间隔时间（秒）
        /// </summary>
        public int ReconnectDelayTime
        {
            get;
            set;
        }


        public ClientConfig()
        {
            this.IsSsl = false;
            this.Host = "127.0.0.1";
            this.Port = 8007;
            this.IdentityProvider = EmptyIdentity.Instance;
            this.ReconnectTimes = 3; //默认重连次数为3
            this.ReconnectDelayTime = 5; //重连间隔时间
        }
    }
}