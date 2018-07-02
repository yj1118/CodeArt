using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]
    public class StyleTargetTypeConvert : TypeConverter
    {
        protected override object GetDefaultValue()
        {
            return null;
        }

        protected override object ConvertTo(string value)
        {
            return ComponentTypeLocator.Locate(value);
        }
    }
}