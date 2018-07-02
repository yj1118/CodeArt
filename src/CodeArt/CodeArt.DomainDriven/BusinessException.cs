using System;

using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    public class BusinessException : UserUIException
    {
        public BusinessException(string message)
            : base(message)
        {
        }

        public BusinessException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
