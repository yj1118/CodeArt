using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 每一个事件条目的执行和回逆都需要满足幂等性
    /// </summary>
    [DebuggerDisplay("EventName:{EventName},EventId:{EventId}")]
    [ObjectRepository(typeof(IEventQueueRepository), CloseMultiTenancy = true)]
    public class EventEntry : EntityObject<EventEntry, int>
    {
        /// <summary>
        /// 得到该事件条目的事件源
        /// </summary>
        [NotEmpty]
        [PropertyRepository(Lazy = true)]
        internal static readonly DomainProperty SourceProperty = DomainProperty.Register<EventEntry, EventEntry>("Source");

        public EventEntry Source
        {
            get
            {
                return GetValue<EventEntry>(SourceProperty);
            }
            private set
            {
                SetValue(SourceProperty, value);
            }
        }

        /// <summary>
        /// 获取该事件条目属于的事件源的事件实例
        /// </summary>
        /// <returns></returns>
        public DomainEvent GetSourceEvent()
        {
            if (this.IsRootSource)
            {
                //根事件源
                var args = DTObject.Create(this.ArgsCode);
                var @event = EventFactory.GetLocalEvent(this.EventName, args, true);
                @event.Entry = this;
                return @event;
            }
            else
            {
                var args = this.Source.GetArgs();
                var @event = EventFactory.GetLocalEvent(this.Source.EventName, args, true);
                @event.Entry = this.Source;
                return @event;
            }
        }

        /// <summary>
        /// 事件名称
        /// </summary>
        [NotEmpty]
        [PropertyRepository()]
        internal static readonly DomainProperty EventNameProperty = DomainProperty.Register<string, EventEntry>("EventName");

        public string EventName
        {
            get
            {
                return GetValue<string>(EventNameProperty);
            }
            private set
            {
                SetValue(EventNameProperty, value);
            }
        }

        /// <summary>
        /// 事件的实例编号
        /// </summary>
        [NotEmpty]
        [PropertyRepository()]
        internal static readonly DomainProperty EventIdProperty = DomainProperty.Register<Guid, EventEntry>("EventId");

        public Guid EventId
        {
            get
            {
                return GetValue<Guid>(EventIdProperty);
            }
            private set
            {
                SetValue(EventIdProperty, value);
            }
        }

        public bool IsLocal
        {
            get
            {
                return EventAttribute.GetTip(this.EventName, false) != null;
            }
        }

        /// <summary>
        /// 该条目是根事件源
        /// </summary>
        public bool IsRootSource
        {
            get
            {
                return this.Source.IsEmpty();
            }
        }


        /// <summary>
        /// 事件状态
        /// </summary>
        [NotEmpty]
        [PropertyRepository()]
        public static readonly DomainProperty StatusProperty = DomainProperty.Register<EventStatus, EventEntry>("Status");

        public EventStatus Status
        {
            get
            {
                return GetValue<EventStatus>(StatusProperty);
            }
            internal set
            {
                SetValue(StatusProperty, value);
            }
        }

        /// <summary>
        /// 条目存储的参数，只有本地事件的条目才存储数据
        /// </summary>
        [PropertyRepository()]
        private static readonly DomainProperty ArgsCodeProperty = DomainProperty.Register<string, EventEntry>("ArgsCode");

        /// <summary>
        /// 队列存储的参数，该参数是基于事件源的，也就是事件源的数据
        /// </summary>
        public string ArgsCode
        {
            get
            {
                return GetValue<string>(ArgsCodeProperty);
            }
            set
            {
                SetValue(ArgsCodeProperty, value);
            }
        }

        /// <summary>
        /// 获得事件条目的参数
        /// </summary>
        /// <returns></returns>
        public DTObject GetArgs()
        {
            if (this.Source.IsEmpty())
            {
                //根条目
                return DTObject.Create(this.ArgsCode);
            }
            else if (this.IsLocal)
            {
                if (string.IsNullOrEmpty(this.ArgsCode))
                {
                    //对于本地事件，如果参数代码为空，表示还没有初始化过，要初始化
                    var source = this.GetSourceEvent();
                    var args = source.GetArgs(this.EventName);
                    this.ArgsCode = args.GetCode(); //保存代码
                    return args;
                }
                else
                    return DTObject.Create(this.ArgsCode); //如果已经有代码，那么直接使用
            }
            else
            {
                //远程事件，直接从源事件中得到参数
                var source = this.GetSourceEvent();
                return source.GetArgs(this.EventName);
            }
        }


        /// <summary>
        /// 获得事件的远程格式
        /// </summary>
        /// <returns></returns>
        internal DTObject GetRemotable(DTObject identity, DTObject args)
        {
            //并没有将本地队列的编号和条目状态传递出去
            var e = DTObject.Create();
            e.SetValue("eventId", this.EventId);
            e.SetValue("eventName", this.EventName);
            e.SetObject("args", args);
            e.SetObject("identity", identity);
            return e;
        }

        internal static EventKey GetEventKey(DTObject @event)
        {
            var eventId = @event.GetValue<Guid>("eventId");
            var eventName = @event.GetValue<string>("eventName");
            return new EventKey(eventId, eventName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EventEntry"></param>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="source"></param>
        /// <param name="argsCode"></param>
        internal EventEntry(EventEntry source, int id, string eventName, Guid eventId, string argsCode)
            : base(id)
        {
            this.Source = source;
            this.ArgsCode = argsCode;
            this.EventName = eventName;
            this.EventId = eventId;
            this.Status = EventStatus.Idle;
            this.OnConstructed();
        }

        [ConstructorRepository()]
        internal EventEntry(int id, string eventName, Guid eventId)
            : base(id)
        {
            this.EventName = eventName;
            this.EventId = eventId;
            this.OnConstructed();
        }

        #region 空对象

        private class EventEntryEmpty : EventEntry
        {
            public EventEntryEmpty()
                : base(0, string.Empty, Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly EventEntry Empty = new EventEntryEmpty();

        #endregion

    }
}
