using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    public class LayoutConverter : TypeConverter
    {
        private LayoutConverter() { }

        protected override object GetDefaultValue()
        {
            return null;
        }

        protected override object ConvertTo(string value)
        {
            switch (value.ToLower())
            {
                case "cell":
                case "layout.cell": return Layout.Cell;
                case "wrap":
                case "layout.wrap": return Layout.Wrap;
                case "hidden":
                case "layout.hidden": return Layout.Hidden;
            }
            return Layout.Inline;
        }

        public static readonly LayoutConverter Instance = new LayoutConverter();

    }
}
