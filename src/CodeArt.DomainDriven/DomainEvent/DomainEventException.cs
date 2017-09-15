using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class DomainEventException : DomainDrivenException
    {
        public DomainEventException()
            : base()
        {
        }

        public DomainEventException(string message)
            : base(message)
        {
        }

        public DomainEventException(string message, Exception innerException)
        : base(message, innerException)
        {
        }

    }
}
