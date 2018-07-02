using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using CodeArt.WPF.UI;
using System.Collections;

namespace CodeArt.WPF.Controls.Playstation
{
    public class SelectItemData : TextItem
    {
        private bool _disabled;

        public bool Disabled
        {
            get
            {
                return _disabled;
            }
            set
            {
                _disabled = value;
                this.MarkPropertyChanged(DisabledPropertyName);
            }
        }

        public SelectItemData(object value)
            : base(value)
        {
        }

        public const string DisabledPropertyName = "Disabled";
    }
}