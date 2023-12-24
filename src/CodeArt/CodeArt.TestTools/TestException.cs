using System;

namespace CodeArt.TestTools
{
    /// <summary>
    /// 
    /// </summary>
    public class TestException : Exception
    {
        public TestException()
            : base()
        {
        }

        public TestException(string message)
            : base(message)
        {
        }

        public TestException(string message, Exception innerException)
            : base(message,innerException)
        {
        }

    }

}
