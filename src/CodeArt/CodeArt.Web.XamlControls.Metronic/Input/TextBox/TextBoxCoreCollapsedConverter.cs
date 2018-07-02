using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class TextBoxCoreCollapsedConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var d = RenderContext.Current.BelongTemplate.TemplateParent as TextBox;
            var type = parameter as string;
            if(type == "1")
            {
                return d.Rows == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return d.Rows > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public readonly static TextBoxCoreCollapsedConverter Instance = new TextBoxCoreCollapsedConverter();
    }
}