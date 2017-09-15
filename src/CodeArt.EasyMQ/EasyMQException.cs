using System;

using CodeArt.Log;

namespace CodeArt.EasyMQ
{
    public class EasyMQException : Exception
    {
        public EasyMQException()
            : base()
        {
        }

        public EasyMQException(string message)
            : base(message)
        {
        }
    }
}
