using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class AppendClassConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var baseClass = parameter as string;
            if (baseClass == null) return value;
            return string.Format("{0} {1}", baseClass, value);
        }

        public readonly static AppendClassConverter Instance = new AppendClassConverter();
    }
}