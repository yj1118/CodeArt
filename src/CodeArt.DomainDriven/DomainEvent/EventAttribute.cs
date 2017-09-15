using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.DomainDriven
{
    public class EventAttribute : Attribute
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        internal Type EventType
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="startUp">是否自动启动</param>
        public EventAttribute(string name)
        {
            this.Name = name;
        }

        public static IEnumerable<EventAttribute> Tips
        {
            get;
            private set;
        }

        public static void Initialize()
        {
            var eventTypes = AssemblyUtil.GetTypesByAttribute<EventAttribute>();

            List<EventAttribute> tips = new List<EventAttribute>(eventTypes.Count());

            foreach(var eventType in eventTypes)
            {
                var tip = AttributeUtil.GetAttribute<EventAttribute>(eventType);
                tip.EventType = eventType;
                tips.Add(tip);
                _typeTips.Add(tip.EventType, tip);
                _nameTips.Add(tip.Name, tip);
            }
            Tips = tips;
        }

        private static Dictionary<Type, EventAttribute> _typeTips = new Dictionary<Type, EventAttribute>();

        public static EventAttribute GetTip(Type eventType, bool throwError)
        {
            EventAttribute tip = null;
            if (_typeTips.TryGetValue(eventType, out tip)) return tip;
            if (throwError) throw new DomainEventException(string.Format(Strings.NoEventAttribute, eventType.FullName));
            return null;
        }

        private static Dictionary<string, EventAttribute> _nameTips = new Dictionary<string, EventAttribute>();

        public static EventAttribute GetTip(string eventName, bool throwError)
        {
            EventAttribute tip = null;
            if (_nameTips.TryGetValue(eventName, out tip)) return tip;
            if (throwError) throw new DomainEventException(string.Format(Strings.NoEvent, eventName));
            return null;
        }
    }
}
