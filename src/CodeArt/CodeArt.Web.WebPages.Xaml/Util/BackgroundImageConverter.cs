using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class BackgroundImageConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var v = value as string;
            return string.IsNullOrEmpty(v) ? string.Empty : string.Format("background-image: url({0});", v);
        }

        public readonly static BackgroundImageConverter Instance = new BackgroundImageConverter();
    }
}