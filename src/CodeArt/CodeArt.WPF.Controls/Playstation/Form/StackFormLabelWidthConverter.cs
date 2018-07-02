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
using System.Windows.Media.Animation;
using System.Globalization;

using CodeArt.WPF.UI;

namespace CodeArt.WPF.Controls.Playstation
{
    public class StackFormLabelWidthConverter : BootstrapWidthConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            parameter = "xs:(*0.3)";
            return base.Convert(value, targetType, parameter, culture);
        }

        public new readonly static StackFormLabelWidthConverter Instance = new StackFormLabelWidthConverter();
    }
}