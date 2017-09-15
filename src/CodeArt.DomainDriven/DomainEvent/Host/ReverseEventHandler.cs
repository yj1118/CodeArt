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
    /// 
    /// </summary>
    [SafeAccess]
    public class ReverseEventHandler : DomainEventHandler
    {
        private ReverseEventHandler()
        {
        }

        public override void Handle(DTObject @event)
        {
            try
            {
                var eventId = @event.GetValue<Guid>(EventEntry.EventIdProperty.Name);
                var eventName = @event.GetValue<string>(EventEntry.EventNameProperty.Name);
                var message = @event.GetValue<string>("message");

                EventRestorer.Restore(eventId, eventName, eventId, message);
            }
            catch (EventRestoreException ex)
            {
                //如果抛出的恢复事件发生的异常，那么意味着发生了很严重的错误
                //此时我们要抛出异常，告诉消息队列不要回复ack,让管理员去处理
                LogWrapper.Default.Fatal(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                //写入日志
                LogWrapper.Default.Fatal(ex);
            }
        }


        public static readonly ReverseEventHandler Instance = new ReverseEventHandler();

    }
}