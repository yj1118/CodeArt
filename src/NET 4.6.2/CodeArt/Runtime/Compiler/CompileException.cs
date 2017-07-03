using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.Runtime
{
    /// <summary>
    /// 编译时错误
    /// </summary>
    [Serializable]
    public class CompileException : Exception
    {
        public CompileException()
        {
        }

        public CompileException(string message)
            : base(message)
        {
        }

        public CompileException(SerializationInfo info, StreamingContext context)
            :base(info,context)
        {
        }
    }
}
