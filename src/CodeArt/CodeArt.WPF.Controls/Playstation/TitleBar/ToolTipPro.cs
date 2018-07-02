using System;
using System.Collections.Generic;
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

namespace CodeArt.WPF.Controls.Playstation
{
    public class ToolTipText : ToolTip
    {
        public ToolTipText()
        {
            this.DefaultStyleKey = typeof(ToolTipText);
            this.DataContext = this;
        }

        public static DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(ToolTipText));

        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

    }
}
