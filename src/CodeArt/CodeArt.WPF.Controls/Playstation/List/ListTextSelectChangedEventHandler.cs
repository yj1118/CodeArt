using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.WPF.Controls.Playstation
{
    public delegate void ListTextSelectChangedEventHandler(object sender, ListTextSelectChangedEventArgs e);

    public sealed class ListTextSelectChangedEventArgs
    {
        public ListTextItem Old
        {
            get;
            private set;
        }

        public ListTextItem New
        {
            get;
            private set;
        }



        public ListTextSelectChangedEventArgs(ListTextItem @new, ListTextItem old)
        {
            this.New = @new;
            this.Old = old;
        }

    }

}