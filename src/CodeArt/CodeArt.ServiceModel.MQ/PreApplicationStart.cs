using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.ServiceModel;
using CodeArt.EasyMQ.RPC;


[assembly: PreApplicationStart(typeof(CodeArt.ServiceModel.MQ.PreApplicationStart),
                                "Initialize",
                                PreApplicationStartPriority.Low)]


namespace CodeArt.ServiceModel.MQ
{
    internal class PreApplicationStart
    {
        private static void Initialize()
        {
            InitRPCServers();
            ServiceProxy.Register(MQServiceProxy.Instance);
        }

        private static void InitRPCServers()
        {
            var services = ServiceAttribute.GetServics();
            foreach(var service in services)
            {
                RPCServer.Initialize(service, MQServiceHandler.Instance);
            }
        }
    }
}
