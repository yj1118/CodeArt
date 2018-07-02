using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.EasyMQ;

namespace CodeArt.EasyMQ.RPC
{
    public static class RPCServer
    {
        public static void Open(string method, IRPCHandler handler)
        {
            var server = _setting.GetFactory().Create(method);
            server.Open(handler);
            RPCEvents.RaiseServerOpened(server, new RPCEvents.ServerOpenedArgs(method));
        }

        public static void Close(string method)
        {
            var server = _setting.GetFactory().Create(method);
            server.Close();
            RPCEvents.RaiseServerClosed(server, new RPCEvents.ServerClosedArgs(method));
        }

        private static FactorySetting<IServerFactory> _setting = new FactorySetting<IServerFactory>(() =>
        {
            var config = EasyMQConfiguration.Current.RPCConfig;
            InterfaceImplementer imp = config.ServerFactoryImplementer;
            if (imp != null)
            {
                return imp.GetInstance<IServerFactory>();
            }
            return null;
        });

        public static void Register(IServerFactory factory)
        {
            _setting.Register(factory);
        }

    }
}