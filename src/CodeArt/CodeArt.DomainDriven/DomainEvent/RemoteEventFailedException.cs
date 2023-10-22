using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class RemoteEventFailedException : DomainDrivenException
    {
        public RemoteEventFailedException(string message)
            : base(message)
        {

        }
    }

    /// <summary>
    /// 远程发生了业务错误
    /// </summary>
    public class RemoteBusinessFailedException : BusinessException
    {
        public RemoteBusinessFailedException(string message)
            : base(message)
        {

        }
    }
}
