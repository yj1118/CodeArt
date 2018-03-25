using System;

using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    public class DomainDrivenException : Exception
    {
        public DomainDrivenException()
            : base()
        {
        }

        public DomainDrivenException(string message)
            : base(message)
        {
        }

        public DomainDrivenException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
