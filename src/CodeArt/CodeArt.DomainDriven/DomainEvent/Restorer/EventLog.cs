using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.AppSetting;
using System.Data;

namespace CodeArt.DomainDriven
{
    [DebuggerDisplay("Status:{Status},EntryCount:{EntryCount}")]
    public class EventLog : DataObject<Guid>
    {
        /// <summary>
        /// 触发该事件队列时系统使用的语言
        /// </summary>
        public string Language
        {
            get;
            private set;
        }


        /// <summary>
        /// 触发该事件队列时系统使用的语言
        /// </summary>
        public long TenantId
        {
            get;
            private set;
        }

        public DTObject GetIdentity()
        {
            var identity = DTObject.Create();
            identity["language"] = this.Language;
            identity["tenantId"] = this.TenantId;
            return identity;
        }

        private EventLogStatus _status;

        public EventLogStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                this.MarkDirty();
            }
        }

        private int _entryCount;

        /// <summary>
        /// 该日志拥有的条目总数
        /// </summary>
        public int EntryCount
        {
            get
            {
                return _entryCount;
            }
            set
            {
                _entryCount = value;
                this.MarkDirty();
            }
        }

        public EventLog()
            : base(Guid.Empty)
        {

        }


        public EventLog(Guid id, string language, long tenantId)
            : base(id)
        {
            this.Status = EventLogStatus.Normal;
            this.Language = language;
            this.TenantId = tenantId;
        }

        protected override void LoadImpl(IDataReader reader)
        {
            this.Id = reader.GetGuid("Id");
            this.Status = (EventLogStatus)reader.GetByte("Status");
            this.Language = reader.GetString("Language");
            this.TenantId = reader.GetInt64("TenantId");
            this.EntryCount = reader.GetInt32("EntryCount");
        }

        #region 写入日志

        /// <summary>
        /// 写入并提交触发事件条目的日志
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="content"></param>
        public static void FlushRaise(EventQueue queue, EventEntry entry)
        {
            var logId = queue.Id;
            //写入日志
            var content = DTObject.Create();
            content["entryId"] = entry.Id;
            EventLog.FlushWrite(logId, EventOperation.Raise, content);
        }

        /// <summary>
        /// 写入并提交被执行完毕的日志
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="entry"></param>
        public static void FlushEnd(Guid queueId)
        {
            var logId = queueId;
            //写入日志
            EventLog.FlushWrite(logId, EventOperation.End, DTObject.Empty);
        }


        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public static void Create(EventQueue queue)
        {
            var log = new EventLog(queue.Id, queue.Language, queue.TenantId);
            var repository = EventLogRepository.Instance;
            repository.Add(log);

            //日志创建好后，立即追加一条日志内容
            Write(log, EventOperation.Start, DTObject.Empty);
        }

        /// <summary>
        /// 查找日志，同一时间只有一个线程可以操作一个队列，所以可以不用锁
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public static EventLog Find(Guid logId)
        {
            var repository = EventLogRepository.Instance;
            return repository.Find(logId, QueryLevel.None);
        }

        public static void FlushUpdate(EventLog log)
        {
            DataContext.NewScope(() =>
            {
                Update(log);
            });
        }

        public static void Update(EventLog log)
        {
            var repository = EventLogRepository.Instance;
            repository.Update(log);
        }

        /// <summary>
        /// 向恢复管理器中写入日志内容
        /// </summary>
        /// <param name="logId">日志编号，同一个日志可以写多份内容，可以根据这些内容执行恢复操作</param>
        /// <param name="name">内容名称</param>
        /// <param name="content">内容</param>
        private static void FlushWrite(Guid logId, EventOperation operation, DTObject content)
        {
            DataContext.NewScope(() =>
            {
                var log = Find(logId);
                Write(log, operation, content);
            });
        }

        private static void Write(EventLog log, EventOperation operation, DTObject content)
        {
            //写入日志条目
            var index = log.EntryCount;
            var contentCode = content.IsEmpty() ? "{}" : content.GetCode();
            var entry = new EventLogEntry(log.Id, operation, contentCode, index);
            var repository = EventLogEntryRepository.Instance;
            repository.Add(entry);

            log.EntryCount++;
            Update(log);
        }

        #endregion

        #region 静态成员

        internal static void Delete(Guid queueId)
        {
            var repository = EventLogRepository.Instance;
            repository.Delete(queueId);
        }

        #endregion





    }
}
