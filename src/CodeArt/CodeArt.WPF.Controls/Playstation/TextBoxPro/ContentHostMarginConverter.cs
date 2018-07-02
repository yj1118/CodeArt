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
using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    internal class ContentHostMarginConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rowHeight = (double)values[0];
            var lineHeight = (double)values[1];
            var m = (rowHeight - lineHeight) / 2;
            return new Thickness(18, m, 18, m);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public readonly static ContentHostMarginConverter Instance = new ContentHostMarginConverter();
    }
}