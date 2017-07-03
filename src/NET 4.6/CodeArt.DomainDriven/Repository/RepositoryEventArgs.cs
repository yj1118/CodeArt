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
        public IRepositoryable Target { get; private set; }

        public BoundedEvent EventType
        {
            get;
            private set;
        }


        public RepositoryEventArgs(IRepositoryable target, BoundedEvent eventType)
        {
            this.Target = target;
            this.EventType = eventType;
        }

    }
}
