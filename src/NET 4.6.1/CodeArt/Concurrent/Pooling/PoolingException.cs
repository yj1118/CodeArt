using System;
using System.Collections.Generic;

namespace CodeArt.Concurrent
{
    public class PoolingException : Exception
    {
        public PoolingException(Exception innerException)
            : base(string.Empty, innerException)
        {

        }

        public PoolingException(string message, Exception innerException)
            : base(message,innerException)
        {

        }

        public PoolingException(string message)
            : base(message)
        {

        }
    }
}
