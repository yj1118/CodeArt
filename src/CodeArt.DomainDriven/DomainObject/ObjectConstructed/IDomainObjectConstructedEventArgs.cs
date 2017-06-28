using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    internal interface IDomainObjectConstructedEventArgs
    {
        DomainObject Source { get; }
    }
}
