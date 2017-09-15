using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.EasyMQ.RPC;

namespace CodeArt.DomainDriven
{
    internal static class RemoteService
    {
        public static DTObject GetObject(RemoteType remoteType, object id)
        {
            var methodName = RemoteServiceName.GetObject(remoteType);
            return RPCClient.Invoke(methodName, (arg) =>
            {
                arg["id"] = id;
                arg["typeName"] = remoteType.FullName;
            });
        }


        public static void NotifyUpdated(RemoteType remoteType, object id)
        {
            var @event = new RemoteObjectUpdated(remoteType, id);
            EventPortal.Publish(@event);
        }

        public static void NotifyDeleted(RemoteType remoteType, object id)
        {
            var @event = new RemoteObjectDeleted(remoteType, id);
            EventPortal.Publish(@event);
        }

        #region 初始化

        internal static void Initialize()
        {
            //开启获取远程对象的RPC服务
            var tips = RemotableAttribute.GetTips();
            foreach (var tip in tips)
            {
                var methodName = RemoteServiceName.GetObject(tip.RemoteType);
                RPCServer.Open(methodName, GetRemoteObject.Instance);
            }

            var remoteTypes = RemoteType.GetTypes();
            foreach (var remoteType in remoteTypes)
            {
                //订阅事件
                RemoteObjectUpdated.Subscribe(remoteType);
                RemoteObjectDeleted.Subscribe(remoteType);
            }
        }

        #endregion

        internal static void Cleanup()
        {
            var tips = RemotableAttribute.GetTips();
            foreach (var tip in tips)
            {
                var methodName = RemoteServiceName.GetObject(tip.RemoteType);
                RPCServer.Close(methodName);
            }

            var remoteTypes = RemoteType.GetTypes();
            foreach (var remoteType in remoteTypes)
            {
                //取消订阅事件
                RemoteObjectUpdated.Cancel(remoteType);
                RemoteObjectDeleted.Cancel(remoteType);
            }
        }
    }
}
