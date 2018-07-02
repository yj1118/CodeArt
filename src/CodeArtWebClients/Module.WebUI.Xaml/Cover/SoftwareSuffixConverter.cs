using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace Module.WebUI.Xaml
{
    public class SoftwareSuffixConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var v = parameter as string;
            var software = Application.Current.FindResource("software");
            return string.IsNullOrEmpty(v) ? software : string.Format("{0}_{1}", v, software);
        }

        public readonly static SoftwareSuffixConverter Instance = new SoftwareSuffixConverter();
    }
}