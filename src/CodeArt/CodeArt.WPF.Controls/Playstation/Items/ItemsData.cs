using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt;
using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    public abstract class ItemsData : ObservableObject, INullProxy
    {
        private int _index;

        public int Index
        {
            get
            {
                return _index;
            }
            internal set
            {
                _index = value;
                this.MarkPropertyChanged("Index");
            }
        }

        public abstract bool IsNull();
    }
}
