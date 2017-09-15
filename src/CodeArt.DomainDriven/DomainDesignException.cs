using System;

using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    [NonLog]
    public class DomainDesignException : DomainDrivenException
    {
        public DomainDesignException()
            : base()
        {
        }

        public DomainDesignException(string message)
            : base(message)
        {
        }
    }
}
