using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 事件锁
    /// </summary>
    [ObjectRepository(typeof(IEventLockRepository))]
    public class EventLock : AggregateRoot<EventLock, Guid>
    {
        [PropertyRepository()]
        private static readonly DomainProperty CreateTimeProperty = DomainProperty.Register<DateTime, EventLock>("CreateTime", (owner) => { return DateTime.Now; });

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

        [ConstructorRepository()]
        public EventLock(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        #region 空对象

        private class EventLockEmpty : EventLock
        {
            public EventLockEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly EventLock Empty = new EventLockEmpty();

        #endregion

        #region 静态成员

        /// <summary>
        /// 获取或创建队列对应的事件锁
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        internal static EventLock GetOrCreate(Guid queueId)
        {
            //先尝试创建队列
            DataContext.NewScope(() =>
            {
                var @lock = Find(queueId, QueryLevel.HoldSingle);
                if (@lock.IsEmpty())
                {
                    Create(queueId);
                }
            }, true);

            return Find(queueId, QueryLevel.Single);
        }

        internal static EventLock Create(Guid queueId)
        {
            var repository = Repository.Create<IEventLockRepository>();
            var @lock = new EventLock(queueId);
            repository.Add(@lock);
            return @lock;
        }

        internal static EventLock Find(Guid queueId, QueryLevel level)
        {
            var repository = Repository.Create<IEventLockRepository>();
            return repository.Find(queueId, level);
        }


        internal static void Delete(EventLock @lock)
        {
            var repository = Repository.Create<IEventLockRepository>();
            repository.Delete(@lock);
        }

        #endregion
    }

}
