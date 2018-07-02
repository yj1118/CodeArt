using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class ChartStyleConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var height = value.ToString();
            return string.Format("width:100%;height:{0}px", height);
        }

        public readonly static ChartStyleConverter Instance = new ChartStyleConverter();
    }
}