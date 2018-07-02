using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.WPF.Controls.Playstation
{
    public sealed class InputValueChangedEventArgs
    {
        public object Value { get; private set; }


        public InputValueChangedEventArgs(object value)
        {
            this.Value = value;
        }

    }
}
