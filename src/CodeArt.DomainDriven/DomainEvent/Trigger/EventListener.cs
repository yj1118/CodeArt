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
    /// 事件触发器
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
            var args = @event.GetObject("args");
            var source = EventFactory.GetLocalEvent(key.EventName, args, true);
            var queueId = key.EventId;
            EventRestorer.UseQueue(queueId,true, (callback) =>
            {
                EventTrigger.Start(key.EventId, source, true, callback); //我们把调用方指定的事件编号作为本地的事件队列编号
            },
            (ex) =>
            {
                //发生了错误就发布出去，通知失败了
                EventTrigger.PublishRaiseFailed(AppContext.Identity, key, ex.GetCompleteInfo()); //再恢复
            });
        }


        /// <summary>
        /// 收到调用结果
        /// </summary>
        /// <param name="event"></param>
        public static void Receive(DTObject @event)
        {
            var key = EventEntry.GetEventKey(@event);
            var queue = EventQueue.FindByEventId(key.EventId);
            if (queue.IsEmpty()) return;//本地无此队列
            var queueId = queue.Id;
            EventRestorer.UseQueue(queueId, false, (callback) =>
             {
                 EventTrigger.Continue(queueId, @event, key, callback);
             }, (ex) =>
             {
                 if (queue.IsSubqueue)
                 {
                    //发生了错误就发布出去，通知失败了
                    EventTrigger.PublishRaiseFailed(AppContext.Identity, key, ex.GetCompleteInfo()); //再恢复
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
            var queue = EventQueue.FindByEventId(key.EventId);
            if (queue.IsEmpty()) return;//本地无此队列
            var queueId = queue.Id;
            EventRestorer.UseQueue(queueId, false, (callback) =>
            {
                EventTrigger.Timeout(queueId, key, callback);
            }, (ex) =>
            {
                if (queue.IsSubqueue)
                {
                    //发生了错误就发布出去，通知失败了
                    EventTrigger.PublishRaiseFailed(AppContext.Identity, key, ex.GetCompleteInfo());
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
            var queue = EventQueue.FindByEventId(key.EventId);
            if (queue.IsEmpty()) return;//本地无此队列
            var queueId = queue.Id;
            EventRestorer.TryRestore(queue.Id, new AskedToReverseException(Strings.AskedToReverseTip), true);
        }

    }
}
