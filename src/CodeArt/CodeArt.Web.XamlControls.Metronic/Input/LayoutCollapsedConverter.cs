using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class LayoutCollapsedConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var layout = (Layout)value;
            var current = parameter.ToString();
            bool show = false;
            switch(current)
            {
                case "Inline": show = layout == Layout.Inline; break;
                case "Wrap": show = layout == Layout.Wrap; break;
                case "Cell": show = layout == Layout.Cell; break;
                case "Hidden": show = layout == Layout.Hidden; break;
            }
            return show ? Visibility.Visible : Visibility.Collapsed;
        }

        public readonly static LayoutCollapsedConverter Instance = new LayoutCollapsedConverter();
    }
}