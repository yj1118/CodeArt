using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 基元类型转换器
    /// </summary>
    [SafeAccess]
    public class VisibilityConverter : TypeConverter
    {
        private VisibilityConverter() { }

        protected override object GetDefaultValue()
        {
            return null;
        }

        protected override object ConvertTo(string value)
        {
            return value.EqualsIgnoreCase("Visibility.Visible") ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly VisibilityConverter Instance = new VisibilityConverter();

    }
}
