using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public interface IEventLockRepository : IRepository<EventLock>
    {
        IEnumerable<EventLock> FindExpireds(int expiredHours);
    }
}
