using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 触发事件的结果
    /// </summary>
    internal class RaiseResultEvent : EventBase
    {
        /// <summary>
        /// 这是事件在请求方的队列编号，不是事件在执行方的队列编号
        /// </summary>
        public Guid QueueId
        {
            get;
            private set;
        }

        public string EventName
        {
            get;
            private set;
        }

        public Guid EventId
        {
            get;
            private set;
        }

        public bool Success
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public string ArgsCode
        {
            get;
            private set;
        }


        internal RaiseResultEvent(Guid queueId, string eventName, Guid eventId, bool success, string message, string argsCode)
            : base(EventUtil.GetRaiseResult(eventName, eventId))
        {
            this.QueueId = queueId;
            this.EventName = eventName;
            this.EventId = eventId;
            this.Success = success;
            this.Message = message;
            this.ArgsCode = argsCode;
        }

        public override DTObject GetRemotable()
        {
            var remotable = DTObject.CreateReusable();
            remotable["queueId"] = this.QueueId;
            remotable["eventName"] = this.EventName;
            remotable["eventId"] = this.EventId;
            remotable["success"] = this.Success;
            remotable["message"] = this.Message;
            remotable["args"] = DTObject.CreateReusable(this.ArgsCode);
            return remotable;
        }
    }
}