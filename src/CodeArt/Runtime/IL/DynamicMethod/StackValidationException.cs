using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 堆栈错误
    /// </summary>
    public class StackValidationException : ApplicationException
    {
        internal StackValidationException(string message)
            : base(message)
        {
        }
    }
}
