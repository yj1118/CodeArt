using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml
{
    public class ZeroDecimalCollapsedConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var show = DataUtil.ToValue<decimal>(value) != 0M;
            if (parameter != null)
            {
                //取反
                return show ? Visibility.Collapsed : Visibility.Visible;
            }
            return show ? Visibility.Visible : Visibility.Collapsed;
        }

        public readonly static ZeroDecimalCollapsedConverter Instance = new ZeroDecimalCollapsedConverter();
    }
}