using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class TextareaConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            if (value == null) return string.Empty;

            var code = ((string)value).Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace(" ", "&nbsp;").Replace("\'", "&#39;").Replace("\"", "&quot;");
            return Regex.Replace(code, @"\r*\n", "<br/>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public readonly static TextareaConverter Instance = new TextareaConverter();
    }
}