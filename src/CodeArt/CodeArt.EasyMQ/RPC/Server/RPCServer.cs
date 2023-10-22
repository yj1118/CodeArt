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
        /// <summary>
        /// 初始化服务，但不启动
        /// </summary>
        /// <param name="method"></param>
        /// <param name="handler"></param>
        public static void Initialize(string method, IRPCHandler handler)
        {
            var server = _setting.GetFactory().Create(method);
            server.Initialize(handler);
            //RPCEvents.RaiseServerOpened(server, new RPCEvents.ServerOpenedArgs(method)); //这里不是开启，不需要通知事件
        }

        /// <summary>
        /// 开启所有服务
        /// </summary>
        /// <param name="method"></param>
        internal static void Open()
        {
            var servers = _setting.GetFactory().GetAll();
            foreach(var server in servers)
            {
                server.Open();
                RPCEvents.RaiseServerOpened(server, new RPCEvents.ServerOpenedArgs(server.Name));
            }
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