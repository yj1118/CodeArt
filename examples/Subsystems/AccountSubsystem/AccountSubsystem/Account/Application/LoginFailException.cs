using System;
using System.Linq;
using System.Collections.Generic;

using CodeArt;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public sealed class LoginFailException : UserUIException
    {
        public LoginFailException(string message)
            : base(message)
        {

        }
    }
}
