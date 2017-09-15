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
    public class SqlEventLogEntryRepository : SqlRepository<EventLogEntry>, IEventLogEntryRepository
    {
        private SqlEventLogEntryRepository() { }


        public IEnumerable<EventLogEntry> FindByReverseOrder(Guid logId)
        {
            return DataContext.Current.Query<EventLogEntry>("log.id=@logId[order by index desc]", (data) =>
            {
                data["logId"] = logId;
            }, QueryLevel.None);
        }


        public static readonly SqlEventLogEntryRepository Instance = new SqlEventLogEntryRepository();

    }
}
