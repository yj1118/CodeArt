using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.Authentication
{
    /// <summary>
    /// 身份认证出错
    /// </summary>
    public class AuthenticationException : Exception
    {
        public AuthenticationException()
            : base()
        {
            
        }

        public AuthenticationException(string message)
            : base(message)
        {
        }
    }
}
