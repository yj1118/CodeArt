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
        /// 是否启用，如果为false,那么需要用 EventHost激活才会启用
        /// </summary>
        public bool IsEnabled
        {
            get;
            internal set;
        }

        /// <summary>
        /// 用于测试：指示该事件是模拟远程服务的
        /// </summary>
        public bool IsMockRemote
        {
            get;
            set;
        }

        public bool RequierdOpen
        {
            get
            {
                return this.IsEnabled && !this.IsMockRemote;
            }
        }


        /// <summary>
        /// 事件名称
        /// </summary>
        /// <param name="name"></param>
        public EventAttribute(string name)
            : this(name,true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="isEnabled">是否启用，如果为false,那么需要用 EventHost激活才会启用</param>
        public EventAttribute(string name,bool isEnabled)
        {
            this.Name = name;
            this.IsEnabled = isEnabled;
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
            var _effectiveName = EventUtil.GetEffectiveName(eventName);
            EventAttribute tip = null;
            if (_nameTips.TryGetValue(_effectiveName, out tip)) return tip;
            if (throwError) throw new DomainEventException(string.Format(Strings.NoEvent, _effectiveName));
            return null;
        }
    }
}
