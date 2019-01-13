using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    [ObjectRepository(typeof(IEventMonitorRepository), CloseMultiTenancy = true)]
    public class EventMonitor : AggregateRoot<EventMonitor, Guid>
    {
        [PropertyRepository()]
        private static readonly DomainProperty InterruptedProperty = DomainProperty.Register<bool, EventMonitor>("Interrupted", false);

        /// <summary>
        /// 监视器是否为中断的，中断的监视器需要被恢复
        /// </summary>
        public bool Interrupted
        {
            get
            {
                return GetValue<bool>(InterruptedProperty);
            }
            set
            {
                SetValue(InterruptedProperty, value);
            }
        }

        [PropertyRepository()]
        private static readonly DomainProperty CreateTimeProperty = DomainProperty.Register<DateTime, EventMonitor>("CreateTime", (owner)=> { return DateTime.Now; });

        /// <summary>
        /// 监视器创建的时间
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

        [ConstructorRepository()]
        public EventMonitor(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        #region 空对象

        private class EventMonitorEmpty : EventMonitor
        {
            public EventMonitorEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly EventMonitor Empty = new EventMonitorEmpty();

        #endregion

        #region 静态成员

        /// <summary>
        /// 获取或创建队列对应的监视器（由于有时间锁，所以此处不加任何锁）
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        internal static EventMonitor GetOrCreate(Guid queueId)
        {
            //先尝试创建队列
            DataContext.NewScope(() =>
            {
                var monitor = Find(queueId);
                if (monitor.IsEmpty())
                {
                    Create(queueId);
                }
            }, true);

            return Find(queueId);
        }

        internal static EventMonitor Create(Guid queueId)
        {
            var repository = Repository.Create<IEventMonitorRepository>();
            var monitor = new EventMonitor(queueId);
            repository.Add(monitor);
            return monitor;
        }

        internal static EventMonitor Find(Guid queueId)
        {
            var repository = Repository.Create<IEventMonitorRepository>();
            return repository.Find(queueId, QueryLevel.None);
        }

        internal static void UpdateAndFlush(EventMonitor monitor)
        {
            DataContext.NewScope(() =>
            {
                Update(monitor);
            }, true);
        }

        internal static void Update(EventMonitor monitor)
        {
            var repository = Repository.Create<IEventMonitorRepository>();
            repository.Update(monitor);
        }

        internal static void Delete(EventMonitor monitor)
        {
            var repository = Repository.Create<IEventMonitorRepository>();
            repository.Delete(monitor);
        }

        /// <summary>
        /// 获得已中断的监视器信息（每次获得10个）
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Guid> Top10Interrupteds()
        {
            var repository = Repository.Create<IEventMonitorRepository>();
            var list = repository.Top10Interrupteds(QueryLevel.None);
            return list.Select((t) => t.Id);
        }


        #endregion

    }

}
