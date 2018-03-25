using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.DTO;
using CodeArt.AppSetting;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    internal static class EventFactory
    {
        /// <summary>
        /// 根据事件条目获取对应的本地定义的事件
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static DomainEvent GetLocalEvent(EventEntry entry, DTObject args, bool throwError)
        {
            return GetLocalEvent(entry.EventName, args, throwError);
        }

        public static DomainEvent GetLocalEvent(string eventName, DTObject args, bool throwError)
        {
            //因为领域事件的处理被事件锁锁了，所以这里不存在并发问题，不需要锁cache
            var tip = EventAttribute.GetTip(eventName, throwError);
            if (tip == null) return null;
            if (!tip.IsEnabled) return null; //如果宿主过滤该事件，那么事件不是本地的
            var @event = (DomainEvent)tip.EventType.GetConstructor(Array.Empty<Type>()).CreateInstance(null);
            if(!args.IsEmpty())
                @event.SetArgs(args);
            return @event;
        }
    }
}
