using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 表示领域事件执行失败，但是成功恢复状态（还原到执行领域事件之前的状态）的异常
    /// </summary>
    public class EventFailedException : DomainDrivenException
    {
        public EventFailedException(Exception reason)
            : base(reason.IsUserUIException() ? string.Empty : Strings.EventErrorAndRestored, reason)
        {

        }
    }
}
