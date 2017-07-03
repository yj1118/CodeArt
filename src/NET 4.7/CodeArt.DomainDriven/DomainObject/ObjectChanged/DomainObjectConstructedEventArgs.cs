using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public sealed class DomainObjectChangedEventArgs : IDomainObjectChangedEventArgs
    {
        public DomainObject Source { get; private set; }

        public DomainObjectChangedEventArgs(DomainObject source)
        {
            this.Source = source;
        }

    }
}
