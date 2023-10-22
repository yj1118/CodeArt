using System;

namespace CodeArt.Security
{
    public class AuthException : UserUIException
    {
        public AuthException(string message)
            : base(message)
        {
        }

        public AuthException()
            : base("authError")
        {
        }
    }
}
