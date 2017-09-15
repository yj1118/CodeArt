using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 与RabbitMQ服务相关的信息
    /// </summary>
    public sealed class Server
    {
        /// <summary>
        /// rabbitMQ的宿主地址，可以使用标准格式host：port（例如host = myhost.com：15672）。
        /// 如果省略端口号，则使用默认的AMQP端口（15672）。
        /// </summary>
        public string Host
        {
            get;
            private set;
        }

        /// <summary>
        /// 虚拟主机名称
        /// </summary>
        public string VirtualHost
        {
            get;
            private set;
        }

        public Server(string host, string virtualHost)
        {
            this.Host = host;
            this.VirtualHost = virtualHost;
        }
    }
}