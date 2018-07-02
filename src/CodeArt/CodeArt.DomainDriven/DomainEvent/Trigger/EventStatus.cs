using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 事件运行状态
    /// </summary>
    public enum EventStatus : byte
    {
        /// <summary>
        /// 闲置中
        /// </summary>
        Idle =1,
        /// <summary>
        /// 正在触发
        /// </summary>
        Raising = 2,
        /// <summary>
        /// 已触发完毕
        /// </summary>
        Raised = 3,
        /// <summary>
        /// 正在回逆
        /// </summary>
        Reversing = 4,
        /// <summary>
        /// 已回逆
        /// </summary>
        Reversed = 5,
        /// <summary>
        /// 执行事件超时，这出现在调用远程事件的时候
        /// </summary>
        TimedOut = 6
    }
}
