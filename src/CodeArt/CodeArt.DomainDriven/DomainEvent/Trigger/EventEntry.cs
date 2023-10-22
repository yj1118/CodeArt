using System;
using System.Collections.Generic;
using System.Data;
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
    public class EventEntry : DataObject<Guid>
    {


        internal Guid SourceId
        {
            get;
            private set;
        }

        /// <summary>
        /// 得到该事件条目的事件源
        /// </summary>
        public EventEntry Source
        {
            get;
            internal set;
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
        public string EventName
        {
            get;
            private set;
        }

        /// <summary>
        /// 事件的实例编号
        /// </summary>
        public Guid EventId
        {
            get
            {
                return this.Id;
            }
        }

        public bool IsLocal
        {
            get
            {
                var tip = EventAttribute.GetTip(this.EventName, false);
                if (tip == null) return false;
                return !tip.IsMockRemote;
            }
        }

        public bool IsMockRemote
        {
            get
            {
                var tip = EventAttribute.GetTip(this.EventName, false);
                if (tip == null) return false;
                return tip.IsMockRemote;
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

        private EventStatus _status;

        /// <summary>
        /// 事件状态
        /// </summary>
        public EventStatus Status
        {
            get
            {
                return _status;
            }
            internal set
            {
                _status = value;
                this.MarkDirty();
            }
        }

        private string _argsCode;

        /// <summary>
        /// 条目存储的参数，只有本地事件的条目才存储数据
        /// 队列存储的参数，该参数是基于事件源的，也就是事件源的数据
        /// </summary>
        public string ArgsCode
        {
            get
            {
                return _argsCode;
            }
            set
            {
                _argsCode = value;
                this.MarkDirty();
            }
        }

        public int OrderIndex
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获得事件条目的参数
        /// </summary>
        /// <returns></returns>
        public DTObject GetArgs()
        {
            if (this.Source.IsEmpty())
            {
                //源事件的源为空，所以这就是源事件对应的条目，直接使用argsCode
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

        public EventEntry()
            : base(Guid.Empty)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EventEntry"></param>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="source"></param>
        /// <param name="argsCode"></param>
        internal EventEntry(EventEntry source, Guid id, string eventName, string argsCode, int orderIndex)
            : base(id)
        {
            this.Source = source;
            this.SourceId = source == null ? Guid.Empty : source.Id;
            this.ArgsCode = argsCode;
            this.EventName = eventName;
            this.OrderIndex = orderIndex;
            this.Status = EventStatus.Idle;
        }

        protected override void LoadImpl(IDataReader reader)
        {
            this.Id = reader.GetGuid("Id");
            this.EventName = reader.GetString("EventName");
            this.ArgsCode = reader.GetString("ArgsCode");
            this.Status = (EventStatus)reader.GetByte("Status");
            this.SourceId = reader.GetGuid("SourceId");
            this.OrderIndex = reader.GetInt32("OrderIndex");
            //事件源由外部加载
        }

        public override bool IsEmpty()
        {
            return this.EventId == Guid.Empty || base.IsEmpty();
        }

        public readonly static EventEntry Empty = new EventEntry(null, Guid.Empty, string.Empty, string.Empty, 0);



        /// <summary>
        /// 专供测试用
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static EventEntry CreateTest(string eventName)
        {
            return new EventEntry(null, Guid.Empty, eventName, string.Empty, -1);
        }

    }
}
