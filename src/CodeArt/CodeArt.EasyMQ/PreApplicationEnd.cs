using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.EasyMQ.RPC;

[assembly: PreApplicationEnd(typeof(CodeArt.EasyMQ.PreApplicationEnd), "Cleanup", PreApplicationStartPriority.Low)]

namespace CodeArt.EasyMQ
{
    internal class PreApplicationEnd
    {
        private static void Cleanup()
        {
            RPCClient.Cleanup();
        }
    }
}
