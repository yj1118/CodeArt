using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    [DebuggerDisplay("Operation:{Operation},ContentCode:{ContentCode},OrderIndex:{OrderIndex}")]
    public class EventLogEntry : DataObject<Guid>
    {
        /// <summary>
        /// 所属日志的编号
        /// </summary>
        public Guid LogId
        {
            get;
            private set;
        }

        public EventOperation Operation
        {
            get;
            private set;
        }

        public string ContentCode
        {
            get;
            private set;
        }

        /// <summary>
        /// 日志条目的写入序号
        /// </summary>
        public int OrderIndex
        {
            get;
            private set;
        }


        private bool _isReversed;

        /// <summary>
        /// 该日志是否已经进行了事件回逆的处理，是为true,否则false
        /// </summary>
        public bool IsReversed
        {
            get
            {
                return _isReversed;
            }
            internal set
            {
                _isReversed = value;
                this.MarkDirty();
            }
        }

        public EventLogEntry()
           : base(Guid.Empty)
        {

        }


        public EventLogEntry(Guid logId, EventOperation operation, string contentCode, int orderIndex)
            : base(Guid.NewGuid())
        {
            this.LogId = logId;
            this.Operation = operation;
            this.ContentCode = contentCode;
            this.OrderIndex = orderIndex;
            this.IsReversed = false;
        }

        protected override void LoadImpl(IDataReader reader)
        {
            this.Id = reader.GetGuid("Id");
            this.LogId = reader.GetGuid("LogId");
            this.Operation = (EventOperation)reader.GetByte("Operation");
            this.ContentCode = reader.GetString("ContentCode");
            this.OrderIndex = reader.GetInt32("OrderIndex");
            this.IsReversed = reader.GetBoolean("IsReversed");
        }


        public static void Update(EventLogEntry entry)
        {
            var repository = EventLogEntryRepository.Instance;
            repository.Update(entry);
        }

        /// <summary>
        /// 删除日志下的所有条目
        /// </summary>
        /// <param name="logId"></param>
        internal static void Deletes(Guid logId)
        {
            var repository = EventLogEntryRepository.Instance;
            repository.Deletes(logId);
        }

    }
}
