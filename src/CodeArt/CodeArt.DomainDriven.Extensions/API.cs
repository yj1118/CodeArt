using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.DTO;
using CodeArt.EasyMQ;
using CodeArt.EasyMQ.Event;
using CodeArt.EasyMQ.RPC;

namespace CodeArt.DomainDriven
{
    public static class API
    {
        const string ClearBuffer = "d:ClearBuffer";
        const string ClearData = "d:ClearData";

        internal static void Initialize()
        {
            //订阅事件
            SubscribeClearBuffer();
            SubscribeClearData();
        }

        #region 清空领域缓冲区

        private static void SubscribeClearBuffer()
        {
            EventPortal.Subscribe(ClearBuffer, ClearBufferHandler.Instance);
        }

        [SafeAccess]
        private class ClearBufferHandler : IEventHandler
        {
            public EventPriority Priority => EventPriority.Medium;

            public void Handle(string eventName, TransferData data)
            {
                var args = data.Info;
                //目前是全部清空
                if (args.Exist("public"))
                {
                    if (args.Exist("all"))
                    {
                        DomainBuffer.Public.Clear();
                        return;
                    }
                }
            }

            public static readonly ClearBufferHandler Instance = new ClearBufferHandler();
        }

        #endregion

        #region 清空数据库

        private static void SubscribeClearData()
        {
            EventPortal.Subscribe(ClearData, ClearDataHandler.Instance);
        }

        [SafeAccess]
        private class ClearDataHandler : IEventHandler
        {
            public EventPriority Priority => EventPriority.Medium;

            public void Handle(string eventName, TransferData data)
            {
                DataPortal.ClearUp();
            }

            public static readonly ClearDataHandler Instance = new ClearDataHandler();
        }

        #endregion

        internal static void Cleanup()
        {
            EventPortal.Cancel(ClearBuffer);
            EventPortal.Cancel(ClearData);
        }
    }
}
