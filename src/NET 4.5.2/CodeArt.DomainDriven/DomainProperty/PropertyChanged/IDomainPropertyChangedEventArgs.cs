using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    internal interface IDomainPropertyChangedEventArgs
    {
        object NewValue { get; }
        object OldValue { get; }
        DomainProperty Property { get; }
    }
}
