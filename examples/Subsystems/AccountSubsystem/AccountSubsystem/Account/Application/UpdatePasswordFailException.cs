using System;
using System.Linq;
using System.Collections.Generic;

using CodeArt;
using CodeArt.DomainDriven;

namespace AccountSubsystem
{
    public sealed class UpdatePasswordFailException : UserUIException
    {
        public UpdatePasswordFailException(string message)
            : base(message)
        {

        }
    }
}
