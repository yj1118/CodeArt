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
    internal class AdjustFontSizeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var adjust = DataUtil.ToValue<double>(parameter);
            var fontSize = (double)value;
            return (double)(fontSize + adjust);
        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public readonly static AdjustFontSizeConverter Instance = new AdjustFontSizeConverter();
    }
}