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
    /// <summary>
    /// 加法转换器
    /// </summary>
    public class UploadFileCollapsedConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (UploadStatus)value;
            if(parameter != null)
            {
                //取反
                return status == UploadStatus.Completed ? Visibility.Collapsed : Visibility.Visible;
            }
            return status == UploadStatus.Completed ? Visibility.Visible : Visibility.Collapsed;
        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public readonly static UploadFileCollapsedConverter Instance = new UploadFileCollapsedConverter();
    }
}