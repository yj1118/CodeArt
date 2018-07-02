using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class RepositoryRollbackEventArgs
    {
        /// <summary>
        /// 需要执行仓储操作的领域对象
        /// </summary>
        public IAggregateRoot Target { get; private set; }

        /// <summary>
        /// 仓储行为
        /// </summary>
        public RepositoryAction Action { get; private set; }

        /// <summary>
        /// 相关仓储
        /// </summary>
        public IPersistRepository Repository { get; private set; }

        public RepositoryRollbackEventArgs(IAggregateRoot target, IPersistRepository repository, RepositoryAction action)
        {
            this.Target = target;
            this.Repository = repository;
            this.Action = action;
        }
    }
}
