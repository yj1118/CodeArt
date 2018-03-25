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
    public class SqlEventMonitorRepository : SqlRepository<EventMonitor>, IEventMonitorRepository
    {
        public IEnumerable<EventMonitor> Top10Interrupteds(QueryLevel level)
        {
            return DataContext.Current.Query<EventMonitor>("interrupted=@interrupted[top 10][order by createTime asc]", (data) =>
            {
                data.Add("@interrupted", true);
            }, level);
        }


        private SqlEventMonitorRepository() { }

        public static readonly SqlEventMonitorRepository Instance = new SqlEventMonitorRepository();
    }
}
