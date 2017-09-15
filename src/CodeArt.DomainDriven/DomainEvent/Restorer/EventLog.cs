using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    [ObjectRepository(typeof(IEventLogRepository))]
    public class EventLog : AggregateRoot<EventLog, Guid>
    {
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
        public static readonly DomainProperty EntryCountProperty = DomainProperty.Register<EventLogStatus, EventLog>("EntryCount");

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
        public EventLog(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        #region 空对象

        private class EventLogEmpty : EventLog
        {
            public EventLogEmpty()
                : base(Guid.Empty)
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


        #region 静态成员

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        internal static void Create(Guid logId)
        {
            DataContext.NewScope(() =>
            {
                var log = new EventLog(logId);
                var repository = Repository.Create<IEventLogRepository>();
                repository.Add(log);
            }, true);
        }

        private static EventLog GetLog(Guid logId)
        {
            var repository = Repository.Create<IEventLogRepository>();
            var log = repository.Find(logId, QueryLevel.Single);
            return log;
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
        internal static void Flush(Guid logId, string name, DTObject content)
        {
            DataContext.NewScope(() =>
            {
                var log = GetLog(logId);

                //写入日志条目
                var index = log.EntryCount;
                var contentCode = content.GetCode();
                var entry = new EventLogEntry(log, name, contentCode, index);
                var repository = Repository.Create<IEventLogEntryRepository>();
                repository.Add(entry);

                log.EntryCount++;
                UpdateLog(log);
            }, true);
        }


        /// <summary>
        /// 根据写入的日志执行恢复操作
        /// </summary>
        /// <param name="logId"></param>
        /// <param name="process"></param>
        internal static void Restore(Guid logId, Func<EventLogEntry, bool> process)
        {
            DataContext.NewScope(() =>
            {
                var log = GetLog(logId);

                var repository = Repository.Create<IEventLogEntryRepository>();
                var entries = repository.FindByReverseOrder(logId);

                bool completed = true;

                foreach (var entry in entries)
                {
                    if (!process(entry))
                    {
                        completed = false;
                        break;
                    }
                }

                log.Status = completed ? EventLogStatus.Reversed : EventLogStatus.Recovering;

                UpdateLog(log);
            }, true);
        }


        #endregion





    }
}
