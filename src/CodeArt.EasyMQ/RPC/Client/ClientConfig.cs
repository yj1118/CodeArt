using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;


namespace CodeArt.EasyMQ.RPC
{
    public class ClientConfig
    {
        /// <summary>
        /// 请求超时时间
        /// </summary>
        public int Timeout
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout">请求超时时间</param>
        private ClientConfig(int timeout)
        {
            this.Timeout = timeout;
        }

        public static readonly ClientConfig Instance = new ClientConfig(EasyMQConfiguration.Current.RPCConfig.ClientTimeout);

    }
}
