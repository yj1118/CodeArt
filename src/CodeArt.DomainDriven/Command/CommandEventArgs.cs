using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public sealed class CommandEventArgs
    {
        private CommandEventArgs()
        {
        }

        internal static CommandEventArgs Instance = new CommandEventArgs();
    }
}
