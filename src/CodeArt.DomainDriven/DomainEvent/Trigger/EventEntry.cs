using System;
using System.Collections.Generic;
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
    [ObjectRepository(typeof(IEventQueueRepository))]
    public class EventEntry : EntityObject<EventEntry, int>
    {
        /// <summary>
        /// 条目所属的队列
        /// </summary>
        [NotEmpty]
        [PropertyRepository(Lazy = true)]
        internal static readonly DomainProperty QueueProperty = DomainProperty.Register<EventQueue, EventEntry>("Queue");

        public EventQueue Queue
        {
            get
            {
                return GetValue<EventQueue>(QueueProperty);
            }
            private set
            {
                SetValue(QueueProperty, value);
            }
        }

        /// <summary>
        /// 得到该事件条目所属的事件源（注意，事件条目的事件源不一定是事件队列的事件源，因为事件队列中的每个事件有可能是本地事件）
        /// </summary>
        [NotEmpty]
        //[PropertyRepository(Lazy = true)]
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
        /// 事件的参数
        /// </summary>
        [PropertyRepository()]
        private static readonly DomainProperty ArgsCodeProperty = DomainProperty.Register<string, EventEntry>("ArgsCode");

        public string ArgsCode
        {
            get
            {
                return GetValue<string>(ArgsCodeProperty);
            }
            internal set
            {
                SetValue(ArgsCodeProperty, value);
            }
        }

        /// <summary>
        /// 获得事件的远程格式
        /// </summary>
        /// <returns></returns>
        public DTObject GetRemotable()
        {
            //并没有将本地队列的编号和条目状态传递出去
            var args = GetArgs();
            var e = DTObject.CreateReusable();
            e.SetValue("queueId", this.Queue.Id);
            e.SetValue("eventId", this.EventId);
            e.SetValue("eventName", this.EventName);
            e.SetObject("args", args);
            return e;
        }

        public const string QueueId = "queueId";

        public DTObject GetArgs()
        {
            return DTObject.CreateReusable(this.ArgsCode);
        }


        /// <summary>
        /// 当事件条目被执行后，触发该回调方法
        /// </summary>
        /// <param name="result"></param>
        internal void Callback(DTObject result)
        {
            this.ArgsCode = result.GetCode();
            //回调
            var entrySource = EventFactory.GetLocalEvent(this.Source, true);
            entrySource.Callback(this.EventName, result);
            this.Source.ArgsCode = entrySource.GetArgs().GetCode(); //此处需要测试在更新queue的时候，是否能更新,todo
        }


        /// <summary>
        /// 该构造函数创建的是远程事件
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="source"></param>
        /// <param name="argsCode"></param>
        internal EventEntry(EventQueue queue, int id, string eventName, EventEntry source, string argsCode)
            : base(id)
        {
            this.Queue = queue;
            this.Source = source;
            this.EventName = eventName;
            this.EventId = Guid.NewGuid();
            this.Status = EventStatus.Idle;
            this.ArgsCode = argsCode;
            this.OnConstructed();
        }

        /// <summary>
        /// 该构造函数创建的是本地事件
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="id"></param>
        /// <param name="eventName"></param>
        /// <param name="argsCode"></param>
        internal EventEntry(EventQueue queue, int id, string eventName, string argsCode)
            : base(id)
        {
            this.Queue = queue;
            this.Source = this; //本地事件的事件源是自己
            this.EventName = eventName;
            this.EventId = Guid.NewGuid();
            this.Status = EventStatus.Idle;
            this.ArgsCode = argsCode;
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
