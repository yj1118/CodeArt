using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class StringCollapsedConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var v = value as string;
            if (parameter != null)
            {
                //取反
                return string.IsNullOrEmpty(v) ? Visibility.Visible : Visibility.Collapsed;
            }
            return string.IsNullOrEmpty(v) ? Visibility.Collapsed : Visibility.Visible;
        }

        public readonly static StringCollapsedConverter Instance = new StringCollapsedConverter();
    }
}