using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]
    public class DrawOriginTypeConverter : TypeConverter
    {
        public DrawOriginTypeConverter() { }

        protected override object GetDefaultValue()
        {
            return DrawOrigin.Current;
        }

        protected override object ConvertTo(string value)
        {
            if (value.Equals("Current", StringComparison.OrdinalIgnoreCase)) return DrawOrigin.Current;
            if (value.Equals("Header", StringComparison.OrdinalIgnoreCase)) return DrawOrigin.Header;
            if (value.Equals("Bottom", StringComparison.OrdinalIgnoreCase)) return DrawOrigin.Bottom;
            throw new XamlException("错误的DrawOrigin格式" + value);
        }
    }


}
