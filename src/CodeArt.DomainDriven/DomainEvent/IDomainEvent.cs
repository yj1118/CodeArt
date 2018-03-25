using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.DomainDriven
{
    public interface IDomainEvent
    {
        /// <summary>
        /// 领域事件可以被触发
        /// </summary>
        void Raise();

        /// <summary>
        /// 领域事件可以被回逆
        /// </summary>
        void Reverse();

    }
}
