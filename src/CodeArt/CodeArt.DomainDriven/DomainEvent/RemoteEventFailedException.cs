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
}
