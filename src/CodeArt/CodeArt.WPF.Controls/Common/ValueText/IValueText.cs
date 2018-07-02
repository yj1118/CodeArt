using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.WPF.Controls
{
    public interface IValueText
    {
        object Value { get; }

        string Text { get; }
    }
}
