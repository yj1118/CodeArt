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
    public class ZeroCollapsedConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var show = DataUtil.ToValue<int>(value) != 0;
            if (parameter != null)
            {
                //取反
                return show ? Visibility.Collapsed : Visibility.Visible;
            }
            return show ? Visibility.Visible : Visibility.Collapsed;
        }

        public readonly static ZeroCollapsedConverter Instance = new ZeroCollapsedConverter();
    }
}