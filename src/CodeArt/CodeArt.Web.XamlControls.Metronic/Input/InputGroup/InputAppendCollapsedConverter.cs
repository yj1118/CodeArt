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
    public class InputAppendCollapsedConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var cell = RenderContext.Current.Target as ITemplateCell;
            var d = cell.BelongTemplate.TemplateParent as Input;
            return d.Append.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public readonly static InputAppendCollapsedConverter Instance = new InputAppendCollapsedConverter();
    }
}