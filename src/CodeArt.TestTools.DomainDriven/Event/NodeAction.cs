using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.TestTools.DomainDriven
{
    public enum NodeAction
    {
        /// <summary>
        /// 成功完成任务
        /// </summary>
        Success = 1,
        /// <summary>
        /// 完成任务失败
        /// </summary>
        Fail = 2,
        /// <summary>
        /// 由于断电等意外故障引起的崩溃
        /// </summary>
        Breakdown = 3
    }
}
