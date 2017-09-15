using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CodeArt.DomainDriven
{
    internal interface IActionMetadata
    {
        /// <summary>
        /// 行为执行过程
        /// </summary>
        ActionProcedure Procedure { get; }

    }
}