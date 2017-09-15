using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountSubsystem
{
    public sealed class AccountException : Exception
    {
        public AccountException()
        {

        }

        public AccountException(string message)
            : base(message)
        {

        }
    }
}
