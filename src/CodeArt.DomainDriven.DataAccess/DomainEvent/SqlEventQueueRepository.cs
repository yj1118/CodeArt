using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    [SafeAccess]
    public class SqlEventQueueRepository : SqlRepository<EventQueue>, IEventQueueRepository
    {
        private SqlEventQueueRepository() { }

        public static readonly SqlEventQueueRepository Instance = new SqlEventQueueRepository();

    }
}
