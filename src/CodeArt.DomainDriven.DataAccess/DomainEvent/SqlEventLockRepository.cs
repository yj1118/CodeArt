using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    [SafeAccess]
    public class SqlEventLockRepository : SqlRepository<EventLock>, IEventLockRepository
    {
        private SqlEventLockRepository() { }

        public static readonly SqlEventLockRepository Instance = new SqlEventLockRepository();

        /// <summary>
        /// 找到过期的
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EventLock> FindExpireds(int expiredHours)
        {
            return DataContext.Current.Query<EventLock>("datediff(hh,createTime,@currentTime)>@hours",(arg)=>
            {
                arg.Add("currentTime",DateTime.Now);
                arg.Add("hours", expiredHours);
            }, QueryLevel.None);
        }
    }
}
