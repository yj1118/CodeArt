using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 触发事件的结果的处理器，当调用事件方获取到执行方的结果时会触发该处理器
    /// </summary>
    [SafeAccess]
    public class RaiseResultEventHandler : DomainEventHandler
    {
        private RaiseResultEventHandler()
        {
        }

        public override void Handle(DTObject @event)
        {
            //以下代码段中，一切没有捕获到的错误，我们都认为是不正常的，会抛出异常到上一级
            //这样就不会ack回复
            try
            {
                EventTrigger.Continue(@event);
            }
            catch(EventRestoreException ex)
            {
                //如果抛出的恢复事件发生的异常，那么意味着发生了很严重的错误
                //此时我们要抛出异常，告诉消息队列不要回复ack,让管理员去处理
                LogWrapper.Default.Fatal(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                LogWrapper.Default.Fatal(ex);
                PublishRaiseFailed(@event, ex);
            }
        }

        private void PublishRaiseFailed(DTObject @event, Exception ex)
        {
            //不抛出异常，消息不会被再次处理
            try
            {
                var queueId = @event.GetValue<Guid>(EventEntry.QueueId);
                var eventId = @event.GetValue<Guid>(EventEntry.EventIdProperty.Name);
                var eventName = @event.GetValue<string>(EventEntry.EventNameProperty.Name);

                EventTrigger.PublishRaiseFailed(queueId, eventName, eventId, ex.Message);
            }
            catch (Exception e)
            {
                //如果发布错误也出错，那么记录日志
                LogWrapper.Default.Fatal(e);
            }
        }

        public static readonly RaiseResultEventHandler Instance = new RaiseResultEventHandler();
    }
}