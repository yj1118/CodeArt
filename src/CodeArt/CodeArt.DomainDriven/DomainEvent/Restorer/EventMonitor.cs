using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class EventMonitor : DataObject<Guid>
    {
        /// <summary>
        /// 监视器是否为中断的，中断的监视器需要被恢复
        /// </summary>
        public bool Interrupted
        {
            get;
            set;
        }

        /// <summary>
        /// 监视器创建的时间
        /// </summary>
        public DateTime CreateTime
        {
            get;
            private set;
        }

        public EventMonitor()
            : base(Guid.Empty)
        {
            
        }

        public EventMonitor(Guid id)
            : base(id)
        {
            this.CreateTime = DateTime.Now;
        }

        protected override void LoadImpl(IDataReader reader)
        {
            this.Id = reader.GetGuid("Id");
            this.Interrupted = reader.GetBoolean("Interrupted");
            this.CreateTime = reader.GetDateTime("CreateTime");
        }


        #region 静态成员

        /// <summary>
        /// 获取或创建队列对应的监视器（由于有时间锁，所以此处不加任何锁）
        /// </summary>
        /// <param name="queueId"></param>
        /// <returns></returns>
        internal static EventMonitor GetOrCreate(Guid queueId)
        {
            //先尝试创建监视器
            DataContext.NewScope(() =>
            {
                var monitor = Find(queueId);
                if (monitor.IsEmpty())
                {
                    Create(queueId);
                }
            });

            return Find(queueId);
        }

        internal static EventMonitor Create(Guid queueId)
        {
            var repository = EventMonitorRepository.Instance;
            var monitor = new EventMonitor(queueId);
            repository.Add(monitor);
            return monitor;
        }

        internal static EventMonitor Find(Guid queueId)
        {
            var repository = EventMonitorRepository.Instance;
            return repository.Find(queueId, QueryLevel.None);
        }

        internal static void FlushUpdate(EventMonitor monitor)
        {
            DataContext.NewScope(() =>
            {
                Update(monitor);
            });
        }

        internal static void Update(EventMonitor monitor)
        {
            var repository = EventMonitorRepository.Instance;
            repository.Update(monitor);
        }

        internal static void Delete(EventMonitor monitor)
        {
            var repository = EventMonitorRepository.Instance;
            repository.Delete(monitor);
        }

        /// <summary>
        /// 获得已中断的监视器信息（每次获得10个）
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Guid> Top10Interrupteds()
        {
            var repository = EventMonitorRepository.Instance;
            return repository.Top10Interrupteds();
        }


        #endregion

    }

}
