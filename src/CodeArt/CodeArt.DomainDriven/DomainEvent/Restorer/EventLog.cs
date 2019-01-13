using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using CodeArt.DomainDriven;
using CodeArt.DTO;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    [DebuggerDisplay("Status:{Status},EntryCount:{EntryCount}")]
    [ObjectRepository(typeof(IEventLogRepository), CloseMultiTenancy = true)]
    public class EventLog : AggregateRoot<EventLog, Guid>
    {
        /// <summary>
        /// 触发该事件队列时系统使用的语言
        /// </summary>
        private static readonly DomainProperty LanguageProperty = DomainProperty.Register<string, EventLog>("Language");

        [PropertyRepository()]
        [ASCIIString()]
        [StringLength(Min = 0, Max = 50)]
        public string Language
        {
            get
            {
                return GetValue<string>(LanguageProperty);
            }
            private set
            {
                SetValue(LanguageProperty, value);
            }
        }

        /// <summary>
        /// 触发该事件队列时系统使用的语言
        /// </summary>
        private static readonly DomainProperty TenantIdProperty = DomainProperty.Register<string, EventLog>("TenantId");

        [PropertyRepository()]
        [ASCIIString()]
        [StringLength(Min = 0, Max = 50)]
        public string TenantId
        {
            get
            {
                return GetValue<string>(TenantIdProperty);
            }
            private set
            {
                SetValue(TenantIdProperty, value);
            }
        }

        public new DTObject GetIdentity()
        {
            var identity = DTObject.Create();
            identity["language"] = this.Language;
            identity["tenantId"] = this.TenantId;
            return identity;
        }

        [PropertyRepository()]
        public static readonly DomainProperty StatusProperty = DomainProperty.Register<EventLogStatus, EventLog>("Status", EventLogStatus.Normal);

        public EventLogStatus Status
        {
            get
            {
                return GetValue<EventLogStatus>(StatusProperty);
            }
            set
            {
                SetValue(StatusProperty, value);
            }
        }

        [PropertyRepository()]
        public static readonly DomainProperty EntryCountProperty = DomainProperty.Register<int, EventLog>("EntryCount");

        /// <summary>
        /// 该日志拥有的条目总数
        /// </summary>
        public int EntryCount
        {
            get
            {
                return GetValue<int>(EntryCountProperty);
            }
            set
            {
                SetValue(EntryCountProperty, value);
            }
        }

        [ConstructorRepository()]
        public EventLog(Guid id, string language,string tenantId)
            : base(id)
        {
            this.Language = language;
            this.TenantId = tenantId;
            this.OnConstructed();
        }

        #region 空对象

        private class EventLogEmpty : EventLog
        {
            public EventLogEmpty()
                : base(Guid.Empty,string.Empty,string.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly EventLog Empty = new EventLogEmpty();

        #endregion

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
            EventLog.WriteAndFlush(logId, EventOperation.Raise, content);
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
            EventLog.WriteAndFlush(logId, EventOperation.End, DTObject.Empty);
        }


        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public static void Create(EventQueue queue)
        {
            var log = new EventLog(queue.Id, queue.Language, queue.TenantId);
            var repository = Repository.Create<IEventLogRepository>();
            repository.Add(log);

            //日志创建好后，立即追加一条日志内容
            Write(log, EventOperation.Start, DTObject.Empty);
        }

        private static EventLog GetLog(Guid logId)
        {
            var repository = Repository.Create<IEventLogRepository>();
            return repository.Find(logId, QueryLevel.Single);
        }

        private static void UpdateLog(EventLog log)
        {
            var repository = Repository.Create<IEventLogRepository>();
            repository.Update(log);
        }

        /// <summary>
        /// 向恢复管理器中写入日志内容
        /// </summary>
        /// <param name="logId">日志编号，同一个日志可以写多份内容，可以根据这些内容执行恢复操作</param>
        /// <param name="name">内容名称</param>
        /// <param name="content">内容</param>
        private static void WriteAndFlush(Guid logId, EventOperation operation, DTObject content)
        {
            DataContext.NewScope(() =>
            {
                var log = GetLog(logId);
                Write(log, operation, content);
            }, true);
        }

        private static void Write(EventLog log, EventOperation operation, DTObject content)
        {
            //写入日志条目
            var index = log.EntryCount;
            var contentCode = content.IsEmpty() ? "{}" : content.GetCode();
            var entry = new EventLogEntry(log, operation, contentCode, index);
            var repository = Repository.Create<IEventLogEntryRepository>();
            repository.Add(entry);

            log.EntryCount++;
            UpdateLog(log);
        }

        #endregion

        #region 静态成员

        /// <summary>
        /// 根据写入的日志执行恢复操作
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="process"></param>
        internal static void Restore(Guid logId, Action<EventLog, EventLogEntry> process)
        {
            DataContext.NewScope(() =>
            {
                var log = GetLog(logId);
                if (log.IsEmpty() || log.Status == EventLogStatus.Reversed) return; //表示已经回逆了

                AppSession.Identity = log.GetIdentity(); //初始化会话身份

                var repository = Repository.Create<IEventLogEntryRepository>();
                var entries = repository.FindByReverseOrder(logId);

                foreach (var entry in entries)
                {
                    if (entry.IsReversed) continue;
                    //为日志条目的回逆创建独立的事务
                    DataContext.NewScope(() =>
                    {
                        process(log, entry);
                        entry.IsReversed = true;
                        EventLogEntry.Update(entry);
                    }, true);
                }

                log.Status = EventLogStatus.Reversed;
                UpdateLog(log);
            }, true);
        }

        internal static void Delete(Guid queueId)
        {
            var repository = Repository.Create<IEventLogRepository>();
            var log = repository.Find(queueId, QueryLevel.None);
            if (!log.IsEmpty()) repository.Delete(log);
        }

        #endregion





    }
}
