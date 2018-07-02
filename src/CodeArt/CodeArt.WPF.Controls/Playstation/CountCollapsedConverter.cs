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
    public class CountCollapsedConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;
            if(parameter != null)
            {
                //反向显示
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Visible;
        }

        public readonly static CountCollapsedConverter Instance = new CountCollapsedConverter();
    }
}