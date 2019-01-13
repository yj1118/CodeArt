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
    public class TextBoxControlClassConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var d = RenderContext.Current.BelongTemplate.TemplateParent as TextBox;
            return string.IsNullOrEmpty(d.Size) ? "form-control" : string.Format("form-control form-control-{0}", d.Size);
        }


        public readonly static TextBoxControlClassConverter Instance = new TextBoxControlClassConverter();
    }
}