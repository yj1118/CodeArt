using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 事件锁
    /// </summary>
    public class EventLock : DataObject<Guid>
    {
        /// <summary>
        /// 创建的时间
        /// </summary>
        public DateTime CreateTime
        {
            get;
            private set;
        }

        public EventLock()
            : base(Guid.Empty)
        {
        }

        public EventLock(Guid id)
            : base(id)
        {
            this.CreateTime = DateTime.Now;
        }

        protected override void LoadImpl(IDataReader reader)
        {
            this.Id = reader.GetGuid("Id");
            this.CreateTime = reader.GetDateTime("CreateTime");
        }

        #region 静态成员

        /// <summary>
        /// 获取或创建队列对应的事件锁
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        internal static EventLock GetOrCreate(Guid queueId)
        {
            DataContext.NewScope(() =>
            {
                var @lock = Find(queueId, QueryLevel.HoldSingle);
                if (@lock.IsEmpty())
                {
                    Create(queueId);
                }
            });
            return Find(queueId, QueryLevel.Single);
        }

        internal static EventLock Create(Guid queueId)
        {
            var repository = EventLockRepository.Instance;
            var @lock = new EventLock(queueId);
            repository.Add(@lock);
            return @lock;
        }

        internal static EventLock Find(Guid queueId, QueryLevel level)
        {
            return EventLockRepository.Instance.Find(queueId, level);
        }

        internal static void Delete(EventLock @lock)
        {
            var repository = EventLockRepository.Instance;
            repository.Delete(@lock.Id);
        }

        #endregion
    }

}
