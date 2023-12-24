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
    internal class UserSecurityMobileModalIdConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            return string.Format("userSecurityModal_mobile_{0}", value.ToString());
        }


        public static UserSecurityMobileModalIdConverter Instance = new UserSecurityMobileModalIdConverter();

    }
}