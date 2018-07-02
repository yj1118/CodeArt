using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 日志状态
    /// </summary>
    public enum EventLogStatus : byte
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 正在恢复
        /// </summary>
        Recovering = 2,
        /// <summary>
        /// 已完成回逆
        /// </summary>
        Reversed = 3
    }
}
