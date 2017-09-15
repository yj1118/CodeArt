using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;

namespace CodeArt.DomainDriven
{
    internal static class EventRestorer
    {
        /// <summary>
        /// 初始化与指定队列相关的日志数据
        /// </summary>
        /// <param name="queue"></param>
        public static void Start(EventQueue queue)
        {
            EventLog.Create(queue.Id);
            //写入日志
            EventLog.Flush(queue.Id, OperationStart, DTObject.Empty);
        }

        /// <summary>
        /// 写入触发事件条目的日志
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="content"></param>
        public static void Raise(EventQueue queue, EventEntry entry)
        {
            var logId = queue.Id;
            //写入日志
            var content = DTObject.CreateReusable();
            content["entryId"] = entry.Id;
            EventLog.Flush(logId, OperationRaise, content);
        }

        /// <summary>
        /// 写入事件被执行完毕的日志
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="entry"></param>
        public static void End(Guid queueId)
        {
            var logId = queueId;
            //写入日志
            EventLog.Flush(logId, OperationEnd, DTObject.Empty);
        }

        #region 恢复

        public static void Restore(Guid queueId, string eventName, Guid eventId, string message)
        {
            try
            {
                //先发布失败了的消息
                EventTrigger.PublishRaiseFailed(queueId, eventName, eventId, message);                                                                   //再恢复
                EventLog.Restore(queueId, Rollback);
            }
            catch (Exception ex)
            {
                throw new EventRestoreException(string.Format(Strings.RecoveryEventFailed, eventName, queueId), ex);
            }
        }


        private static bool Rollback(EventLogEntry logEntry)
        {
            var queueId = logEntry.Log.Id;
            if (logEntry.Operation == OperationRaise)
            {
                var content = DTObject.CreateReusable(logEntry.ContentCode);

                var entryId = content.GetValue<int>("entryId");
                var raiseType = content.GetValue<string>("raiseType");

                DataContext.Using(() =>
                {
                    var queue = EventQueue.Find(queueId);
                    if (queue.IsEmpty()) return;
                    var entry = queue.GetEntry(entryId);
                    if (!entry.IsEmpty())
                    {
                        Reverse(entry);
                    }
                    EventQueue.Update(queue);
                }, true);
            }
            else if (logEntry.Operation == OperationStart)
            {
                DataContext.Using(() =>
                {
                    EventQueue.Delete(queueId);
                }, true);
            }

            return true;
        }

        private static void Reverse(EventEntry entry)
        {
            if (entry.IsEmpty() || 
                entry.Status == EventStatus.Reversed)  //已回逆的不用回逆，保证幂等性
                return;

            if (entry.IsLocal)
            {
                if (entry.Status != EventStatus.Raised) return; //没有完成触发的不用回逆
                ReverseLocalEvent(entry);
            }
            else
            {
                ReverseRemoteEvent(entry);
            }
        }

        private static void ReverseLocalEvent(EventEntry entry)
        {
            var local = EventFactory.GetLocalEvent(entry, false);
            entry.Status = EventStatus.Reversing;
            local.Reverse();
            entry.Status = EventStatus.Reversed;
        }

        private static void ReverseRemoteEvent(EventEntry entry)
        {
            entry.Status = EventStatus.Reversing;
            //发布“回逆事件”的事件
            var reverseEventName = EventUtil.GetReverse(entry.EventName);
            EventPortal.Publish(new RaiseEvent(reverseEventName, entry.GetRemotable()));
            //注意，与“触发事件”不同，我们不需要等待回逆的结果，只用传递回逆的消息即可
            entry.Status = EventStatus.Raised;
        }


        #endregion

        private const string OperationStart = "Start";
        private const string OperationRaise = "Raise";
        private const string OperationEnd = "End";
    }
}
