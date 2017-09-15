using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{

    [ObjectRepository(typeof(IEventLogEntryRepository))]
    public class EventLogEntry : AggregateRoot<EventLogEntry, Guid>
    {
        [PropertyRepository(Lazy = true)]
        public static readonly DomainProperty LogProperty = DomainProperty.RegisterCollection<EventLog, EventLogEntry>("Log");

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
        private static readonly DomainProperty OperationProperty = DomainProperty.RegisterCollection<string, EventLogEntry>("Operation");

        public string Operation
        {
            get
            {
                return GetValue<string>(OperationProperty);
            }
            private set
            {
                SetValue(OperationProperty, value);
            }
        }

        [PropertyRepository()]
        private static readonly DomainProperty ContentCodeProperty = DomainProperty.RegisterCollection<string, EventLogEntry>("ContentCode");

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
        private static readonly DomainProperty IndexProperty = DomainProperty.Register<int, EventLogEntry>("Index");

        public int Index
        {
            get
            {
                return GetValue<int>(IndexProperty);
            }
            private set
            {
                SetValue(IndexProperty, value);
            }
        }

        public EventLogEntry(EventLog log, string operation, string contentCode, int index)
            : base(Guid.NewGuid())
        {
            this.Operation = operation;
            this.ContentCode = contentCode;
            this.Index = index;
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public EventLogEntry(Guid id, string operation, string contentCode, int index)
            : base(id)
        {
            this.Operation = operation;
            this.ContentCode = contentCode;
            this.Index = index;
            this.OnConstructed();
        }

        #region 空对象

        private class EventLogEntryEmpty : EventLogEntry
        {
            public EventLogEntryEmpty()
                : base(Guid.Empty, string.Empty, string.Empty, 0)
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

    }
}
