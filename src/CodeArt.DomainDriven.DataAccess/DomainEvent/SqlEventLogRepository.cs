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
    public class SqlEventLogRepository : SqlRepository<EventLog>, IEventLogRepository
    {
        private SqlEventLogRepository() { }

        public static readonly SqlEventLogRepository Instance = new SqlEventLogRepository();
    }
}
