using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.AppSetting;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.EasyMQ;
using CodeArt.EasyMQ.Event;
using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 专门用于处理领域消息的处理器
    /// </summary>
    [SafeAccess]
    public abstract class DomainMessageHandler : IEventHandler
    {
        public EventPriority Priority => EventPriority.Medium;

        public void Handle(string eventName, TransferData data)
        {
            var msgId = data.Info.GetValue<Guid>("DomainMessageId", Guid.Empty);
            var sync = data.Info.GetValue<bool>("Sync", false);

            if(sync)
            {
                string error = null;
                try
                {
                    if (!this.IsIgnore(msgId))
                    {
                        Invoke(eventName, data);
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    throw ex;
                }
                finally
                {
                    SendResult(msgId, error);  //表示已完成
                }
            }
            else
            {
                if (!this.IsIgnore(msgId))
                {
                    Invoke(eventName, data);
                }
            }
        }

        private void SendResult(Guid msgId, string error)
        {
            DTObject result = DTObject.Create();
            if (!string.IsNullOrEmpty(error))
            {
                result["error"] = error;
            }

            var resultEventName = DomainMessage.GetRaiseResultEventName(msgId);//消息队列的事件名称
            EventPortal.Publish(resultEventName, result);
        }

        private void Invoke(string eventName, TransferData data)
        {
            Process(eventName, data);
            //完成后，要标记该消息已处理，为幂等性考虑，todo
        }

        protected abstract void Process(string eventName, TransferData data);

        private bool IsIgnore(Guid msgId)
        {
            if (msgId == Guid.Empty) return true; //没有编号，不处理

            //目前硬编码是不忽略，todo....
            return false;
        }
    }
}
