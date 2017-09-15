using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public interface IEventLogEntryRepository : IRepository<EventLogEntry>
    {
        /// <summary>
        /// 根据日志编号，以逆序的方式查找日志条目，请注意，该方法不用提供锁版本的实现
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        IEnumerable<EventLogEntry> FindByReverseOrder(Guid logId);
    }
}
