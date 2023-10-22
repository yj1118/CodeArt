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
    public class RPCServerFactory : IServerFactory
    {
        public IServer Create(string method)
        {
            return _getServer(method.ToLower());
        }

        public IEnumerable<IServer> GetAll()
        {
            return _servers;
        }

        private static List<IServer> _servers = new List<IServer>();

        private static Func<string, IServer> _getServer = LazyIndexer.Init<string, IServer>((method) =>
        {
            var server = new RPCServer(method);
            _servers.Add(server);
            return server;
        });

        public static readonly RPCServerFactory Instance = new RPCServerFactory();

    }
}