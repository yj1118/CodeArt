using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.WPF.Controls
{
    public class TextItem : ObservableObject,IValueText
    {
        public object Value
        {
            get;
            private set;
        }

        private string _text;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                this.MarkPropertyChanged("Text");
            }
        }
        
        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            internal set
            {
                _isSelected = value;
                this.MarkPropertyChanged(IsSelectedPropertyName);
            }
        }

        public TextItem(object value)
        {
            this.Value = value;
        }

        public const string IsSelectedPropertyName = "IsSelected";

    }
}
