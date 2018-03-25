using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    internal class UserSecurityMobileFormIdConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            return string.Format("userSecurityMobileForm_{0}", value.ToString());
        }


        public static UserSecurityMobileFormIdConverter Instance = new UserSecurityMobileFormIdConverter();

    }
}