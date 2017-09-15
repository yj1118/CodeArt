using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.EasyMQ;
using CodeArt.EasyMQ.RPC;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 为事件提供广播服务的广播器
    /// </summary>
    [SafeAccess]
    public class RPCClientFactory : IClientFactory
    {
        public IClient Create(ClientConfig config)
        {
            var millisecondsTimeout = config.Timeout * 1000;
            return new RPCClient(millisecondsTimeout);
        }

        public static readonly RPCClientFactory Instance = new RPCClientFactory();

    }
}