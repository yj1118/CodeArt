using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    [ObjectRepository(typeof(IEventQueueRepository))]
    public class EventQueue : AggregateRoot<EventQueue, Guid>
    {
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
        /// 该队列的事件源
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
            FillEntries(source, entries);
            return new DomainCollection<EventEntry>(EntriesProperty, entries);
        }

        private void FillEntries(DomainEvent source, List<EventEntry> entries)
        {
            //将事件源转为条目
            var argsCode = source.GetArgs().GetCode();
            var sourceEntry = new EventEntry(this, entries.Count, source.EventName, argsCode);

            //将前置事件转为事件条目
            foreach (var eventName in source.PreEvents)
            {
                var local = EventFactory.GetLocalEvent(eventName, DTObject.Empty, false);
                if (local == null)
                {
                    var entry = new EventEntry(this, entries.Count, eventName, sourceEntry, DTObject.EmptyCode);
                    entries.Add(entry);
                }
                else
                {
                    FillEntries(local, entries);
                }
            }

            entries.Add(sourceEntry); //最后添加事件源的条目
        }


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
            //为闲置的事件条目获取参数
            SetArgs(idle);
            return idle;
        }

        /// <summary>
        /// 为事件条目设置参数
        /// </summary>
        /// <param name="entry"></param>
        private static void SetArgs(EventEntry entry)
        {
            var source = EventFactory.GetLocalEvent(entry.Source, true);
            var args = source.GetArgs(entry.EventName);
            entry.ArgsCode = args.GetCode();
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


        public EventQueue(Guid id, DomainEvent source)
            : base(id)
        {
            this.EntriesImpl = GetEntriesFromSource(source);
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public EventQueue(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        #region 空对象

        private class EventQueueEmpty : EventQueue
        {
            public EventQueueEmpty()
                : base(Guid.Empty)
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

        internal static EventQueue Create(Guid queueId, DomainEvent source)
        {
            var repository = Repository.Create<IEventQueueRepository>();
            var queue = new EventQueue(queueId, source);
            repository.Add(queue);
            return queue;
        }

        internal static EventQueue Find(Guid queueId)
        {
            var repository = Repository.Create<IEventQueueRepository>();
            return repository.Find(queueId, QueryLevel.Single);
        }

        internal static void Update(EventQueue queue)
        {
            var repository = Repository.Create<IEventQueueRepository>();
            repository.Update(queue);
        }

        internal static void Delete(Guid queueId)
        {
            var repository = Repository.Create<IEventQueueRepository>();
            var queue = repository.Find(queueId, QueryLevel.Single);
            if (!queue.IsEmpty()) repository.Delete(queue);
        }

        #endregion
    }
}
