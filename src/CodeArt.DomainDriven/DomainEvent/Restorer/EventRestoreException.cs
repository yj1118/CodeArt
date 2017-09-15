using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class EventRestoreException : DomainEventException
    {
        public EventRestoreException()
            : base()
        {
        }

        public EventRestoreException(string message)
            : base(message)
        {
        }

        public EventRestoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
