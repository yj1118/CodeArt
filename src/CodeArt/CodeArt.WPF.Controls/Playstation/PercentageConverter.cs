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
    internal class PercentageConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double value = (double)values[0];
            double width = (double)values[1];
            return width * value;
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public readonly static PercentageConverter Instance = new PercentageConverter();
    }
}