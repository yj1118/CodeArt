using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt
{
    /// <summary>
    /// 代表错误会呈现给用户看
    /// </summary>
    public class UserUIException : Exception
    {
        public UserUIException()
            : base()
        {
        }

        public UserUIException(string message)
            : base(message)
        {
        }

        public UserUIException(string message, Exception innerException)
        : base(message, innerException)
        {
        }

    }
}
