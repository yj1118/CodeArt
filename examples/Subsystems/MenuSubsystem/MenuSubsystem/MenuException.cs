using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace MenuSubsystem
{
    public class MenuException : DomainDrivenException
    {
        public MenuException()
            : base()
        {
        }

        public MenuException(string message)
            : base(message)
        {
        }
    }
}
