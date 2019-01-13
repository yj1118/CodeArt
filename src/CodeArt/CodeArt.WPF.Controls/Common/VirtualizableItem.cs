using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.WPF.Controls
{
    public class VirtualizableItem : ObservableObject,IVirtualizable
    {
        private bool _isVisible;

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
                this.MarkPropertyChanged(IsVisiblePropertyName);
            }
        }

        private double _height;

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                this.MarkPropertyChanged(HeightPropertyName);
            }
        }

        public int Index
        {
            get;
            private set;
        }

        public VirtualizableItem(int index)
        {
            this.Index = index;
        }

        public const string IsVisiblePropertyName = "IsVisible";
        public const string HeightPropertyName = "Height";
    }
}
