using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 表示领域事件执行失败，并且没有成功恢复，这是一个严重的异常
    /// </summary>
    public class EventErrorException : DomainDrivenException
    {
        public EventErrorException(Exception reason)
            : base(Strings.EventErrotTip, reason)
        {

        }


        public EventErrorException(string message)
           : base(message)
        {

        }

        public static EventErrorException CreateNoQueue(Guid eventId)
        {
            return new EventErrorException(string.Format("没有找到事件条目{0}所在的事件队列", eventId));
        }

    }

}
