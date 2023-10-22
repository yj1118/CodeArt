using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.EasyMQ.RPC;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 远程对象的获取，更新、删除的状态同步，全部与身份有关，同一个身份的数据互通，因此
    /// 在RPC中获取对象，会提交身份；更新、删除的订阅和发布的事件名称都是以身份名称为后缀的
    /// </summary>
    internal static class RemoteService
    {
        public static DTObject GetObject(AggregateRootDefine define, object id)
        {
            var remoteType = define.RemoteType;
            var methodName = RemoteActionName.GetObject(remoteType);
            return RPCClient.Invoke(methodName, (arg) =>
            {
                arg["id"] = id;
                arg["typeName"] = remoteType.FullName;
                arg["schemaCode"] = define.MetadataSchemaCode;
                arg["identity"] = AppContext.Identity; //没有直接使用session的身份是因为有可能服务点只为一个项目（一个身份）而架设
            }).Info;
        }


        public static void NotifyUpdated(RemoteType remoteType, object id)
        {
            var arg = CreateEventArg(remoteType, id);
            var eventName = RemoteObjectUpdated.GetEventName(remoteType);
            EventPortal.Publish(eventName, arg);
        }

        public static void NotifyDeleted(RemoteType remoteType, object id)
        {
            var arg = CreateEventArg(remoteType, id);
            var eventName = RemoteObjectDeleted.GetEventName(remoteType);
            EventPortal.Publish(eventName, arg);
        }

        private static DTObject CreateEventArg(RemoteType remoteType, object id)
        {
            var arg = DTObject.Create();
            arg["identity"] = AppContext.Identity;
            arg["typeName"] = remoteType.FullName;
            arg["id"] = id;
            return arg;
        }

        #region 初始化

        internal static void Initialize()
        {
            //开启获取远程对象的RPC服务
            var tips = RemotableAttribute.GetTips();
            foreach (var tip in tips)
            {
                var methodName = RemoteActionName.GetObject(tip.RemoteType);
                RPCServer.Initialize(methodName, GetRemoteObject.Instance);
            }

            //订阅事件
            SubscribeEvents();
        }

        internal static void Cleanup()
        {
            var tips = RemotableAttribute.GetTips();
            foreach (var tip in tips)
            {
                var methodName = RemoteActionName.GetObject(tip.RemoteType);
                RPCServer.Close(methodName);
            }

            //取消订阅
            CancelEvents();
        }

        #region 订阅/取消订阅事件


        private static void SubscribeEvents()
        {
            var remoteTypes = RemoteType.GetTypes();
            foreach (var remoteType in remoteTypes)
            {
                RemoteObjectUpdated.Subscribe(remoteType);
                RemoteObjectDeleted.Subscribe(remoteType);
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        private static void CancelEvents()
        {
            var remoteTypes = RemoteType.GetTypes();
            foreach (var remoteType in remoteTypes)
            {
                //取消订阅对象被修改和删除的事件
                RemoteObjectUpdated.Cancel(remoteType);
                RemoteObjectDeleted.Cancel(remoteType);
            }
        }

        #endregion



        #endregion

    }
}
