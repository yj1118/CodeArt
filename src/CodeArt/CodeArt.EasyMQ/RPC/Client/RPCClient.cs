using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.AppSetting;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.EasyMQ.RPC
{
    public static class RPCClient
    {
        public static TransferData Invoke(string method, Action<DTObject> fillArg)
        {
            var arg = DTObject.Create();
            fillArg(arg);
            return Invoke(method, arg);
        }

        public static TransferData Invoke(string method, Action<dynamic> fillArg)
        {
            var arg = DTObject.Create();
            fillArg(arg);
            return Invoke(method, arg);
        }

        public static TransferData Invoke(string method, DTObject arg)
        {
            TransferData result;
            using (var temp = _pool.Borrow())
            {
                var client = temp.Item;
                result = client.Invoke(method, arg);
            }
            return result;
        }



        #region 获取客户端实例

        private static Pool<IClient> _pool = new Pool<IClient>(() =>
        {
            var factory = _setting.GetFactory();
            return factory.Create(ClientConfig.Instance);
        }, (client, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                client.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 20 //闲置时间20秒
        });

        private static FactorySetting<IClientFactory> _setting = new FactorySetting<IClientFactory>(() =>
        {
            var config = EasyMQConfiguration.Current.RPCConfig;
            InterfaceImplementer imp = config.ClientFactoryImplementer;
            if (imp != null)
            {
                return imp.GetInstance<IClientFactory>();
            }
            return null;
        });

        public static void Register(IClientFactory factory)
        {
            _setting.Register(factory);
        }


        #endregion

        /// <summary>
        /// 释放客户端资源
        /// </summary>
        internal static void Cleanup()
        {
            _pool.Dispose();
        }

    }
}