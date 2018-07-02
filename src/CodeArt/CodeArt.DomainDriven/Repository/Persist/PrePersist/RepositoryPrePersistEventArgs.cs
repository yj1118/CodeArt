using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class RepositoryPrePersistEventArgs
    {
        /// <summary>
        /// 需要执行仓储操作的领域对象
        /// </summary>
        public IAggregateRoot Target { get; private set; }

        /// <summary>
        /// 获取或设置是否允许继续执行后续操作
        /// </summary>
        public bool Allow { get; set; }

        /// <summary>
        /// 仓储行为
        /// </summary>
        public RepositoryAction Action { get; private set; }

        public RepositoryPrePersistEventArgs(IAggregateRoot target, RepositoryAction action)
        {
            this.Target = target;
            this.Action = action;
            this.Allow = true; //默认是允许执行后续操作的
        }

    }
}
