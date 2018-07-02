using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    internal interface IDependencyPropertyChangedEventArgs
    {
        object NewValue { get; }
        object OldValue { get; }
        DependencyProperty Property { get; }
    }
}
