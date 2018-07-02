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
    public class RadioCoreClassConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            if (value == null || (bool)value) return "m-radio-list";
            return "m-radio-inline";
        }

        public readonly static RadioCoreClassConverter Instance = new RadioCoreClassConverter();
    }
}