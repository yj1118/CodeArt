using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    internal class InputClassConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var align = value as string;

            StringBuilder className = new StringBuilder("form-control");
            if (align != "left") className.AppendFormat(" text-{0}", align);
            return className.ToString();
        }

        public static readonly InputClassConverter Instance = new InputClassConverter();
    }
}
