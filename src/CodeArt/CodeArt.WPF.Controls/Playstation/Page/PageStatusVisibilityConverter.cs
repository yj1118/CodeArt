﻿using System;
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
    internal class PageStatusVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //用Hidden而不是Collapsed,因为Collapsed不会载入组件，导致后台代码在动态查找时得不到想要的结果，所以我们换成Hidden
            var status = (PageStatus)value;
            if (parameter != null)
            {
                return status == PageStatus.Loading ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                return status == PageStatus.Loading ? Visibility.Hidden : Visibility.Visible;
            }
        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public readonly static PageStatusVisibilityConverter Instance = new PageStatusVisibilityConverter();
    }
}