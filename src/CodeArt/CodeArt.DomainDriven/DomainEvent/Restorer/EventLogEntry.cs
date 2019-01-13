using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    [DebuggerDisplay("Operation:{Operation},ContentCode:{ContentCode},OrderIndex:{OrderIndex}")]
    [ObjectRepository(typeof(IEventLogEntryRepository), CloseMultiTenancy = true)]
    public class EventLogEntry : AggregateRoot<EventLogEntry, Guid>
    {
        [PropertyRepository(Lazy = true)]
        public static readonly DomainProperty LogProperty = DomainProperty.Register<EventLog, EventLogEntry>("Log");

        public EventLog Log
        {
            get
            {
                return GetValue<EventLog>(LogProperty);
            }
            private set
            {
                SetValue(LogProperty, value);
            }
        }

        [PropertyRepository()]
        private static readonly DomainProperty OperationProperty = DomainProperty.Register<EventOperation, EventLogEntry>("Operation");

        public EventOperation Operation
        {
            get
            {
                return GetValue<EventOperation>(OperationProperty);
            }
            private set
            {
                SetValue(OperationProperty, value);
            }
        }

        [PropertyRepository()]
        private static readonly DomainProperty ContentCodeProperty = DomainProperty.Register<string, EventLogEntry>("ContentCode");

        public string ContentCode
        {
            get
            {
                return GetValue<string>(ContentCodeProperty);
            }
            private set
            {
                SetValue(ContentCodeProperty, value);
            }
        }

        /// <summary>
        /// 日志条目的写入序号
        /// </summary>
        [PropertyRepository()]
        private static readonly DomainProperty OrderIndexProperty = DomainProperty.Register<int, EventLogEntry>("OrderIndex");

        public int OrderIndex
        {
            get
            {
                return GetValue<int>(OrderIndexProperty);
            }
            private set
            {
                SetValue(OrderIndexProperty, value);
            }
        }

        /// <summary>
        /// 该日志是否已经进行了事件回逆的处理，是为true,否则false
        /// </summary>
        [PropertyRepository()]
        private static readonly DomainProperty IsReversedProperty = DomainProperty.Register<bool, EventLogEntry>("IsReversed");

        /// <summary>
        /// 该日志是否已经进行了事件回逆的处理，是为true,否则false
        /// </summary>
        public bool IsReversed
        {
            get
            {
                return GetValue<bool>(IsReversedProperty);
            }
            internal set
            {
                SetValue(IsReversedProperty, value);
            }
        }

        public EventLogEntry(EventLog log, EventOperation operation, string contentCode, int orderIndex)
            : base(Guid.NewGuid())
        {
            this.Log = log;
            this.Operation = operation;
            this.ContentCode = contentCode;
            this.OrderIndex = orderIndex;
            this.IsReversed = false;
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public EventLogEntry(Guid id, EventOperation operation, string contentCode, int orderIndex, bool isReversed)
            : base(id)
        {
            this.Operation = operation;
            this.ContentCode = contentCode;
            this.OrderIndex = orderIndex;
            this.IsReversed = isReversed;
            this.OnConstructed();
        }

        #region 空对象

        private class EventLogEntryEmpty : EventLogEntry
        {
            public EventLogEntryEmpty()
                : base(Guid.Empty, 0, string.Empty, 0, false)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly EventLogEntry Empty = new EventLogEntryEmpty();

        #endregion

        public static void Update(EventLogEntry entry)
        {
            var repository = Repository.Create<IEventLogEntryRepository>();
            repository.Update(entry);
        }

        /// <summary>
        /// 删除日志下的所有条目
        /// </summary>
        /// <param name="logId"></param>
        internal static void Deletes(Guid logId)
        {
            var repository = Repository.Create<IEventLogEntryRepository>();
            var entries = repository.FindByReverseOrder(logId);
            foreach(var entry in entries)
            {
                repository.Delete(entry);
            }
        }

    }
}
