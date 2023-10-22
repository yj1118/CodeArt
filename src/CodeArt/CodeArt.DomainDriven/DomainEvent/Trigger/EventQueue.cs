using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.AppSetting;
using CodeArt.Runtime;
using CodeArt.DTO;
using CodeArt.Util;
using System.Data;

namespace CodeArt.DomainDriven
{
    public class EventQueue : DataObject<Guid>
    {
        /// <summary>
        /// 触发该事件队列时系统使用的语言
        /// </summary>
        public string Language
        {
            get;
            private set;
        }


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


        /// <summary>
        /// 该队列是否为子队列，也就是说，是属于某个事件条目调用而产生的队列
        /// </summary>
        public bool IsSubqueue
        {
            get;
            private set;
        }

        #region 事件条目


        /// <summary>
        /// 事件条目
        /// </summary>
        private List<EventEntry> EntriesImpl
        {
            get;
            set;
        }

        public IEnumerable<EventEntry> Entries
        {
            get
            {
                return this.EntriesImpl;
            }
            internal set
            {
                this.EntriesImpl = new List<EventEntry>(value);
            }
        }

        /// <summary>
        /// 该队列的事件源，事件源的编号等于队列编号
        /// </summary>
        public EventEntry Source
        {
            get
            {
                var entry = this.EntriesImpl.FirstOrDefault((e) => e.Id == this.Id);
                if (entry == null) throw new DomainEventException(string.Format("没有找到队列{0}的事件源", this.Id));
                return entry;
            }
        }

        /// <summary>
        /// 创建的时间
        /// </summary>
        public DateTime CreateTime
        {
            get;
            private set;
        }


        private List<EventEntry> GetEntriesFromSource(DomainEvent source)
        {
            //将前置事件转为事件条目
            var entries = new List<EventEntry>();

            //持久化source的参数
            var argsCode = source.GetArgs().GetCode();
            //源事件的编号要与队列编号相同;源事件的源事件是空
            var sourceEntry = new EventEntry(EventEntry.Empty, this.Id, source.EventName, argsCode, -1); //-1是占位用的
            FillPreEntries(source, sourceEntry, entries);
            {
                sourceEntry.OrderIndex = entries.Count;  //这里填写真实的序号
                entries.Add(sourceEntry); //添加事件源的条目
            }
            FillProEntries(source, sourceEntry, entries);
#if DEBUG
            CheckEntries(source, entries);
#endif
            return entries;
        }

        /// <summary>
        /// 在测试下，效验事件条目是否正确
        /// </summary>
        /// <param name="source"></param>
        /// <param name="actual"></param>
        private static void CheckEntries(DomainEvent source, IEnumerable<EventEntry> entries)
        {
            var expectEntries = source.TestGetEventEntries();
            if (expectEntries == null) return; //表示不验证
            var expect = expectEntries.ToList();
            var actual = entries.ToList();
            if (expect.Count() != actual.Count()) throw new BusinessException("事件条目生成错误");
            var count = expect.Count();
            for(var i=0;i<count;i++)
            {
                var a = actual[i];
                var e = expect[i];
                if (a.EventName != e.EventName)
                {
                    throw new BusinessException("事件条目生成错误");
                }
            }
        }


        /// <summary>
        /// 填充前置事件条目
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ownerEntry"></param>
        /// <param name="entries"></param>
        /// <param name="idCount"></param>
        private void FillPreEntries(DomainEvent source, EventEntry sourceEntry, List<EventEntry> entries)
        {
            foreach (var eventName in source.PreEvents)
            {
                FillEntry(sourceEntry, entries, eventName);
            }
        }


        /// <summary>
        /// 填充后置事件条目
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ownerEntry"></param>
        /// <param name="entries"></param>
        /// <param name="idCount"></param>
        private void FillProEntries(DomainEvent source, EventEntry sourceEntry, List<EventEntry> entries)
        {
            foreach (var eventName in source.ProEvents)
            {
                FillEntry(sourceEntry, entries, eventName);
            }
        }

        private void FillEntry(EventEntry sourceEntry, List<EventEntry> entries, string eventName)
        {
            const string argsCode = ""; //条目事件的参数在队列没执行的时候，是空的
            var local = EventFactory.GetLocalEvent(eventName, DTObject.Empty, false);
            if (local == null)
            {
                var entry = new EventEntry(sourceEntry, Guid.NewGuid(), eventName, argsCode, entries.Count);
                entries.Add(entry);
            }
            else
            {
                //将本地事件转为条目
                var localEntry = new EventEntry(sourceEntry, Guid.NewGuid(), eventName, argsCode, entries.Count);
                FillPreEntries(local, localEntry, entries);
                entries.Add(localEntry);
                FillProEntries(local, localEntry, entries);
            }
        }

        #endregion

        /// <summary>
        /// 指示队列是否正在运行中，也就是事件条目是否处于执行或者回逆中
        /// </summary>
        public bool IsRunning
        {
            get
            {
                var runningEntry = this.EntriesImpl.FirstOrDefault((e) =>
                {
                    return e.Status == EventStatus.Raising || e.Status == EventStatus.Reversing;
                });
                return runningEntry != null;
            }
        }

        /// <summary>
        /// 队列已成功完成
        /// </summary>
        public bool IsSucceeded
        {
            get
            {
                var noRaised = this.EntriesImpl.FirstOrDefault((e) =>
                {
                    return e.Status != EventStatus.Raised;
                });
                return noRaised == null;
            }
        }


        /// <summary>
        /// 获得一个需要被触发的事件条目
        /// </summary>
        /// <returns></returns>
        public EventEntry GetPreRaise()
        {
            if (this.IsRunning) return EventEntry.Empty;
            var idle = this.EntriesImpl.FirstOrDefault((e) =>
            {
                return e.Status == EventStatus.Idle;
            });
            if (idle == null) return EventEntry.Empty;
            return idle;
        }


        /// <summary>
        /// 根据事件编号找到事件条目
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public EventEntry GetEntry(Guid eventId)
        {
            var entry = this.EntriesImpl.FirstOrDefault((e) =>
            {
                return e.EventId == eventId;
            });
            return entry == null ? EventEntry.Empty : entry;
        }



        public EventQueue()
            : base(Guid.Empty)
        {

        }


        public EventQueue(Guid id, DomainEvent source, bool isSubqueue, dynamic identity)
            : base(id)
        {
            this.CreateTime = DateTime.Now;
            this.EntriesImpl = GetEntriesFromSource(source);
            this.IsSubqueue = isSubqueue;
            this.Language = identity.Language ?? string.Empty;
            this.TenantId = identity.TenantId ?? 0;
        }

        protected override void LoadImpl(IDataReader reader)
        {
            this.Id = reader.GetGuid("Id");
            this.Language = reader.GetString("Language");
            this.CreateTime = reader.GetDateTime("CreateTime");
            this.IsSubqueue = reader.GetBoolean("IsSubqueue");
            this.TenantId = reader.GetInt64("TenantId");
            //EntriesImpl信息由外部仓储加载
        }

        [ConstructorRepository()]
        public EventQueue(Guid id,string language)
            : base(id)
        {
            this.Language = language;
        }

        #region 静态成员

        internal static EventQueue Create(Guid queueId, DomainEvent source, bool isSubqueue, dynamic identity)
        {
            var repository = EventQueueRepository.Instance;
            var queue = new EventQueue(queueId, source, isSubqueue, identity);
            repository.Add(queue);
            return queue;
        }

        internal static EventQueue Find(Guid queueId)
        {
            var repository = EventQueueRepository.Instance;
            return repository.Find(queueId, QueryLevel.None); //因为已经有事件锁了，所以此处不必锁
        }

        /// <summary>
        /// 根据事件编号得到队列编号
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        internal static EventQueue FindByEventId(Guid eventId)
        {
            var repository = EventQueueRepository.Instance;
            return repository.FindByEventId(eventId, QueryLevel.None);
        }

        internal static void FlushUpdate(EventQueue queue)
        {
            DataContext.NewScope(() =>
            {
                Update(queue);
            });
        }

        internal static void Update(EventQueue queue)
        {
            var repository = EventQueueRepository.Instance;
            repository.Update(queue);
        }

        internal static void Delete(Guid queueId)
        {
            var repository = EventQueueRepository.Instance;
            repository.Delete(queueId);
        }

        #endregion
    }
}
