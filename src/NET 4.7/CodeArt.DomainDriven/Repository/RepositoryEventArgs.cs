using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class RepositoryEventArgs
    {
        /// <summary>
        /// 需要执行仓储操作的领域对象
        /// </summary>
        public IAggregateRoot Target { get; private set; }

        public StatusEventType EventType
        {
            get;
            private set;
        }


        public RepositoryEventArgs(IAggregateRoot target, StatusEventType eventType)
        {
            this.Target = target;
            this.EventType = eventType;
        }

    }
}
