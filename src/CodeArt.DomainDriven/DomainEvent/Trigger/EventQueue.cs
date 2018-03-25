using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.AppSetting;
using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    [ObjectRepository(typeof(IEventQueueRepository))]
    public class EventQueue : AggregateRoot<EventQueue, Guid>
    {
        /// <summary>
        /// 触发该事件队列时系统使用的语言
        /// </summary>
        private static readonly DomainProperty LanguageProperty = DomainProperty.Register<string, EventQueue>("Language");

        [PropertyRepository()]
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

        public new DTObject GetIdentity()
        {
            var identity = DTObject.CreateReusable();
            identity["language"] = this.Language;
            return identity;
        }

        /// <summary>
        /// 该队列是否为子队列，也就是说，是属于某个事件条目调用而产生的队列
        /// </summary>
        [PropertyRepository()]
        private static readonly DomainProperty IsSubqueueProperty = DomainProperty.Register<bool, EventQueue>("IsSubqueue");

        public bool IsSubqueue
        {
            get
            {
                return GetValue<bool>(IsSubqueueProperty);
            }
            private set
            {
                SetValue(IsSubqueueProperty, value);
            }
        }

        #region 事件条目

        /// <summary>
        /// 事件条目
        /// </summary>
        [PropertyRepository()]
        public static readonly DomainProperty EntriesProperty = DomainProperty.RegisterCollection<EventEntry, EventQueue>("Entries");

        private DomainCollection<EventEntry> EntriesImpl
        {
            get
            {
                return GetValue<DomainCollection<EventEntry>>(EntriesProperty);
            }
            set
            {
                SetValue(EntriesProperty, value);
            }
        }

        public IEnumerable<EventEntry> Entries
        {
            get
            {
                return this.EntriesImpl;
            }
        }

        /// <summary>
        /// 该队列的事件源，事件源的编号等于队列编号
        /// </summary>
        public EventEntry Source
        {
            get
            {
                return this.EntriesImpl.Last();
            }
        }


        private DomainCollection<EventEntry> GetEntriesFromSource(DomainEvent source)
        {
            //将前置事件转为事件条目
            var entries = new List<EventEntry>();

            var idCount = 1; //编号计数
            //持久化source的参数
            var argsCode = source.GetArgs().GetCode();
            //源事件的编号要与队列编号相同;源事件的源事件是空
            var sourceEntry = new EventEntry(EventEntry.Empty, idCount, source.EventName, this.Id, argsCode);
            FillPreEntries(source, sourceEntry, entries, ref idCount);
            entries.Add(sourceEntry); //最后添加事件源的条目
            return new DomainCollection<EventEntry>(EntriesProperty, entries);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="ownerEntry"></param>
        /// <param name="entries"></param>
        /// <param name="idCount"></param>
        private void FillPreEntries(DomainEvent source, EventEntry sourceEntry, List<EventEntry> entries, ref int idCount)
        {
            const string argsCode = ""; //条目事件的参数在队列没执行的时候，是空的
            foreach (var eventName in source.PreEvents)
            {
                var local = EventFactory.GetLocalEvent(eventName, DTObject.Empty, false);
                if (local == null)
                {
                    idCount++;
                    var entry = new EventEntry(sourceEntry, idCount, eventName, Guid.NewGuid(), argsCode);
                    entries.Add(entry);
                }
                else
                {
                    //将本地事件转为条目
                    idCount++;
                    var localEntry = new EventEntry(sourceEntry, idCount, eventName, Guid.NewGuid(), argsCode);
                    FillPreEntries(local, localEntry, entries, ref idCount);
                    entries.Add(localEntry);
                }
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


        public EventEntry GetEntry(int entryId)
        {
            var entry = this.EntriesImpl.FirstOrDefault((e) =>
            {
                return e.Id == entryId;
            });
            return entry == null ? EventEntry.Empty : entry;
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

        [PropertyRepository()]
        private static readonly DomainProperty CreateTimeProperty = DomainProperty.Register<DateTime, EventQueue>("CreateTime", (owner) => { return DateTime.Now; });

        /// <summary>
        /// 创建的时间
        /// </summary>
        public DateTime CreateTime
        {
            get
            {
                return GetValue<DateTime>(CreateTimeProperty);
            }
            private set
            {
                SetValue(CreateTimeProperty, value);
            }
        }


        public EventQueue(Guid id, DomainEvent source, bool isSubqueue, dynamic identity)
            : base(id)
        {
            this.EntriesImpl = GetEntriesFromSource(source);
            this.IsSubqueue = isSubqueue;
            this.Language = identity.Language ?? string.Empty;
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public EventQueue(Guid id,string language)
            : base(id)
        {
            this.Language = language;
            this.OnConstructed();
        }

        #region 空对象

        private class EventQueueEmpty : EventQueue
        {
            public EventQueueEmpty()
                : base(Guid.Empty, string.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly EventQueue Empty = new EventQueueEmpty();

        #endregion


        #region 静态成员

        internal static EventQueue Create(Guid queueId, DomainEvent source, bool isSubqueue, dynamic identity)
        {
            var repository = Repository.Create<IEventQueueRepository>();
            var queue = new EventQueue(queueId, source, isSubqueue, identity);
            repository.Add(queue);
            return queue;
        }

        internal static EventQueue Find(Guid queueId)
        {
            var repository = Repository.Create<IEventQueueRepository>();
            return repository.Find(queueId, QueryLevel.None); //因为已经有事件锁了，所以此处不必锁
        }

        /// <summary>
        /// 根据事件编号得到队列编号
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        internal static EventQueue FindByEventId(Guid eventId)
        {
            var repository = Repository.Create<IEventQueueRepository>();
            return repository.FindByEventId(eventId, QueryLevel.None);
        }

        internal static void UpdateAndFlush(EventQueue queue)
        {
            DataContext.NewScope(() =>
            {
                Update(queue);
            }, true);
        }

        internal static void Update(EventQueue queue)
        {
            var repository = Repository.Create<IEventQueueRepository>();
            repository.Update(queue);
        }

        internal static void Delete(Guid queueId)
        {
            var repository = Repository.Create<IEventQueueRepository>();
            var queue = repository.Find(queueId, QueryLevel.None);
            if (!queue.IsEmpty()) repository.Delete(queue);
        }

        #endregion
    }
}
