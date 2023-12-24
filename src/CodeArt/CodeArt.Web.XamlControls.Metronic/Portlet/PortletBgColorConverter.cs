using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class PortletBgColorConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            return string.Format("background-color: {0};", value);

        }

        public readonly static PortletBgColorConverter Instance = new PortletBgColorConverter();
    }
}