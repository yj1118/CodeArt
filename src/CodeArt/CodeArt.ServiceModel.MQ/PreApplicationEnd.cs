using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.EasyMQ.RPC;

[assembly: PreApplicationEnd(typeof(CodeArt.ServiceModel.MQ.PreApplicationEnd), "Cleanup", PreApplicationStartPriority.Low)]

namespace CodeArt.ServiceModel.MQ
{
    internal class PreApplicationEnd
    {
        private static void Cleanup()
        {
            CleanupRPCServers();
        }

        private static void CleanupRPCServers()
        {
            var services = ServiceAttribute.GetServics();
            foreach (var service in services)
            {
                RPCServer.Close(service);
            }
        }

    }
}
