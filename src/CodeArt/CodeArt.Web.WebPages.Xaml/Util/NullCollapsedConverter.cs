using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class NullCollapsedConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var show = value != null;
            if (parameter != null)
            {
                //取反
                return show ? Visibility.Collapsed : Visibility.Visible;
            }
            return show ? Visibility.Visible : Visibility.Collapsed;
        }

        public readonly static NullCollapsedConverter Instance = new NullCollapsedConverter();
    }
}