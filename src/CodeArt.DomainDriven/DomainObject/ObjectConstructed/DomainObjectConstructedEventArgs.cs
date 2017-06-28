using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public sealed class DomainObjectConstructedEventArgs : IDomainObjectConstructedEventArgs
    {
        public DomainObject Source { get; private set; }

        public DomainObjectConstructedEventArgs(DomainObject source)
        {
            this.Source = source;
        }

    }
}
