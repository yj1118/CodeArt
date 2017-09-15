using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class RepositoryPersistedEventArgs
    {
        /// <summary>
        /// 需要执行仓储操作的领域对象
        /// </summary>
        public IAggregateRoot Target { get; private set; }


        /// <summary>
        /// 仓储行为
        /// </summary>
        public RepositoryAction Action { get; private set; }

        public RepositoryPersistedEventArgs(IAggregateRoot target, RepositoryAction action)
        {
            this.Target = target;
            this.Action = action;
        }
    }
}
