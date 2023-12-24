using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 基元类型转换器
    /// </summary>
    public class PrimitiveTypeConverter : TypeConverter
    {
        private PrimitiveTypeConverter() { }

        protected override object GetDefaultValue()
        {
            return null;
        }

        protected override object ConvertTo(string value)
        {
            return null;
        }

        public static PrimitiveTypeConverter Instance = new PrimitiveTypeConverter();

    }
}
