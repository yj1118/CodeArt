using System;

using CodeArt.Log;

namespace CodeArt.RabbitMQ
{
    public class RabbitMQException : Exception
    {
        public RabbitMQException()
            : base()
        {
        }

        public RabbitMQException(string message)
            : base(message)
        {
        }
    }
}
