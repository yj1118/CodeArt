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
    public class ListViewItemPortletColorConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var color = value.ToString();
            var baseClass = parameter.ToString();
            if (string.IsNullOrEmpty(color)) return baseClass;
            return string.Format("{0} m-badge--{1}", baseClass, color);
        }

        public readonly static ListViewItemPortletColorConverter Instance = new ListViewItemPortletColorConverter();
    }
}