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
    public class RadioLabelClassConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var color = value as string;
            return string.IsNullOrEmpty(color) ? "m-radio" : string.Format("m-radio m-radio--state-{0}", color);
        }

        public readonly static RadioLabelClassConverter Instance = new RadioLabelClassConverter();
    }
}