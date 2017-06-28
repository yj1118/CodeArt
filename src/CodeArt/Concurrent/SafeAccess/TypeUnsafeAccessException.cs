using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// 类型不是并发访问安全的
    /// </summary>
    public class TypeUnsafeAccessException : Exception
    {
        public TypeUnsafeAccessException()
            : base()
        {
        }

        public TypeUnsafeAccessException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType">实际类型</param>
        public TypeUnsafeAccessException(Type objType)
            : base(string.Format(Strings.TypeUnsafeConcurrentAccess, objType.FullName))
        {
        }
    }
}
