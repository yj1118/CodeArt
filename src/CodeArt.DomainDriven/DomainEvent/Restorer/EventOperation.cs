using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public enum EventOperation : byte
    {
        /// <summary>
        /// 事件的开始点
        /// </summary>
        Start = 1,
        /// <summary>
        /// 事件条目被执行
        /// </summary>
        Raise = 2,
        /// <summary>
        ///  事件被执行完毕
        /// </summary>
        End = 3
    }
}
