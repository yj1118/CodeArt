using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Util;

namespace Module.QuestionAnswer
{
    public class TypeCollapsedConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var type = (string)value;
            var current = parameter.ToString();
            return type.EqualsIgnoreCase(current) ? Visibility.Visible : Visibility.Collapsed;
        }

        public readonly static TypeCollapsedConverter Instance = new TypeCollapsedConverter();
    }
}