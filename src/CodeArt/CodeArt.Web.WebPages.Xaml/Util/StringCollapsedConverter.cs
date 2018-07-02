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
            return string.IsNullOrEmpty(v) ? Visibility.Collapsed : Visibility.Visible;
        }

        public readonly static StringCollapsedConverter Instance = new StringCollapsedConverter();
    }
}