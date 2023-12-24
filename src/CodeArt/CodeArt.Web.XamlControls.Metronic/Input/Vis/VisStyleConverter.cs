using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Concurrent;
using CodeArt.Web.XamlControls.Metronic;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class VisStyleConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var height = value as string;
            return string.IsNullOrEmpty(height) ? string.Empty : string.Format("height:{0}", height);
        }

        public readonly static VisStyleConverter Instance = new VisStyleConverter();
    }
}