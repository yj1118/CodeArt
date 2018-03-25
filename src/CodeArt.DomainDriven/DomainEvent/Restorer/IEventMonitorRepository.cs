using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public interface IEventMonitorRepository : IRepository<EventMonitor>
    {
        /// <summary>
        /// 获得已中断的监视器信息,
        /// 每次获取10个，按照时间排正序，越早的排越前面（也就是中断时间越长的排前面）
        /// </summary>
        /// <returns></returns>
        IEnumerable<EventMonitor> Top10Interrupteds(QueryLevel level);

    }
}
