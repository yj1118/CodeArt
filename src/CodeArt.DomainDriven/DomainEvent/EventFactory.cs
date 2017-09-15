using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    internal static class EventFactory
    {
        /// <summary>
        /// 根据事件条目获取对应的本地定义的事件
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static DomainEvent GetLocalEvent(EventEntry entry, bool throwError)
        {
            return GetLocalEvent(entry.EventName, entry.GetArgs(), throwError);
        }

        public static DomainEvent GetLocalEvent(string eventName, DTObject args, bool throwError)
        {
            var tip = EventAttribute.GetTip(eventName, throwError);
            if (tip == null) return null;
            if (EventHost.IsFiltered(tip)) return null; //如果宿主过滤该事件，那么事件不是本地的
            var @event = (DomainEvent)tip.EventType.GetConstructor(Array.Empty<Type>()).CreateInstance(null);
            @event.SetArgs(args);
            return @event;
        }
    }
}
