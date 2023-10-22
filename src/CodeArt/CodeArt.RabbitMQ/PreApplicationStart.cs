using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;

[assembly: PreApplicationStart(typeof(CodeArt.RabbitMQ.PreApplicationStart),
                                "Initialize",
                                PreApplicationStartPriority.High)]

namespace CodeArt.RabbitMQ
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            EasyMQ.Event.EventPortal.Register(EventPublisherFactory.Instance);
            EasyMQ.Event.EventPortal.Register(EventSubscriberFactory.Instance);

            EasyMQ.RPC.RPCClient.Register(RPCClientFactory.Instance);
            EasyMQ.RPC.RPCServer.Register(RPCServerFactory.Instance);
        }
    }
}