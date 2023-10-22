using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

using CodeArt.Log;

namespace CodeArt
{
    /// <summary>
    /// 代表错误会呈现给用户看
    /// </summary>
    [NonLog]
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

    public static class ExceptionExtensions
    {
        public static bool IsUserUIException(this Exception ex)
        {
            var temp = ex;
            while (temp != null)
            {
                if (temp is UserUIException) return true;
                temp = temp.InnerException;
            }
            return false;
        }
    }

}
