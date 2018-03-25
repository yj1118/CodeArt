using System;

using CodeArt.Log;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 对于rabbit的异常,大多数不需要追踪调用栈就可以分析问题，所以继承自UserUIException
    /// </summary>
    public class RabbitMQException : UserUIException
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
