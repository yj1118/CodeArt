using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.EasyMQ.Event;
using CodeArt.EasyMQ.RPC;


[assembly: ProApplicationStarted(typeof(CodeArt.EasyMQ.ProApplicationStarted),
                                "Initialized")]


namespace CodeArt.EasyMQ
{
    internal class ProApplicationStarted
    {
        private static void Initialized()
        {
            RPCServer.Open(); //初始化之后，再启动服务

            EventPortal.StartUp(); //初始化之后，再启动订阅器
        }
    }
}
