using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.EasyMQ.Event;
using CodeArt.Runtime;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 事件侦听
    /// </summary>
    internal static class EventListener
    {
        /// <summary>
        /// 接入事件，也就是收到对方的调用事件
        /// </summary>
        /// <param name="event"></param>
        public static void Accept(DTObject @event)
        {
            var key = EventEntry.GetEventKey(@event);
            try
            {
                var args = @event.GetObject("args");
                var source = EventFactory.GetLocalEvent(key.EventName, args, true);
                var queueId = key.EventId; //将外界的事件编号作为本地的队列编号
                EventProtector.UseNewQueue(queueId, (callback) =>
                {
                    EventTrigger.Start(key.EventId, source, true, callback); //我们把调用方指定的事件编号作为本地的事件队列编号
                },
                (ex) =>
                {
                    //发生了错误就发布出去，通知失败了
                    EventTrigger.PublishRaiseFailed(AppContext.Identity, key, ex);
                });
            }
            catch(Exception ex)
            {
                //发生了错误就发布出去
                EventTrigger.PublishRaiseFailed(AppContext.Identity, key, ex);
            }
           
        }

        /// <summary>
        /// 收到调用结果
        /// </summary>
        /// <param name="event"></param>
        public static void Receive(DTObject @event)
        {
            var key = EventEntry.GetEventKey(@event);
            var queue = GetQueueInfo(key);
            EventProtector.UseExistedQueue(queue.Id, (callback) =>
             {
                 EventTrigger.Continue(queue.Id, @event, key, callback);
             }, (ex) =>
             {
                 if (queue.IsSubqueue)
                 {
                    //发生了错误就发布出去，通知失败了
                    EventTrigger.PublishRaiseFailed(AppContext.Identity, key, ex); //再恢复
                }
                 else
                 {
                    //如果不是外界调用而引起的事件，那么出现错误后只用恢复即可，不需要做额外的处理，内部会处理好
                 }
             });
        }

        /// <summary>
        /// 调用远程事件超时
        /// </summary>
        /// <param name="event"></param>
        public static void Timeout(EventKey key)
        {
            var queue = GetQueueInfo(key);
            var queueId = queue.Id;
            EventProtector.UseExistedQueue(queueId, (callback) =>
            {
                EventTrigger.Timeout(queueId, key, callback);
            }, (ex) =>
            {
                if (queue.IsSubqueue)
                {
                    //发生了错误就发布出去，通知失败了
                    EventTrigger.PublishRaiseFailed(AppContext.Identity, key, ex);
                }
                else
                {
                    //如果不是外界调用而引起的事件，那么出现错误后只用恢复即可，不需要做额外的处理，内部会处理好
                }
            });
        }


        /// <summary>
        /// 被要求回逆，这意味着收到回逆的指令
        /// </summary>
        /// <param name="event"></param>
        public static void AskedToReverse(DTObject @event)
        {
            var key = EventEntry.GetEventKey(@event);
            var queue = GetQueueInfo(key);
            var queueId = queue.Id;
            EventProtector.TryRestore(queue.Id, new AskedToReverseException(Strings.AskedToReverseTip), true);
        }

        private static (Guid Id, bool IsSubqueue) GetQueueInfo(EventKey key)
        {
            (Guid Id, bool IsSubqueue) result = default((Guid, bool));
            DataContext.NewScope(() =>
            {
                var queue = EventQueue.FindByEventId(key.EventId);
                if (queue.IsEmpty())
                {
                    var ex = DomainEvent.OnErrorNoQueue(key.EventId);
                    throw ex;//本地无此队列
                }
                result = (queue.Id, queue.IsSubqueue);
            });
            return result;
        }
    }
}
