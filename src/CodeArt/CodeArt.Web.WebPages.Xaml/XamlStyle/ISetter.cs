using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml
{
    internal interface ISetter
    {
        DependencyProperty Property { get; set; }
        object Value { get; set; }
    }
}
