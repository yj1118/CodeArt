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
    internal class ListViewItemTitleBindConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var titleField = value as string;
            if (string.IsNullOrEmpty(titleField)) return "{}";
            return "{binds:{text:'" + titleField + "'}}";
        }

        public readonly static ListViewItemTitleBindConverter Instance = new ListViewItemTitleBindConverter();
    }
}
