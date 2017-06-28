using System;

using CodeArt.Log;

namespace CodeArt.DomainDriven
{
    public class DataContextException : Exception
    {
        public DataContextException()
            : base()
        {
        }

        public DataContextException(string message)
            : base(message)
        {
        }
    }
}
