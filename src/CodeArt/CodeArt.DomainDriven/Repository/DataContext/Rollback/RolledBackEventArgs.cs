using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class RolledBackEventArgs
    {
        public DataContext Context
        {
            get;
            private set;
        }

        public RolledBackEventArgs(DataContext context)
        {
            this.Context = context;
        }
    }
}
