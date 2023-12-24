using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class AlertClassConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var alert = RenderContext.Current.BelongTemplate.TemplateParent as Alert;
            var baseClass = alert.GetClass();
            return string.Format("{0} {1}", baseClass, value);

        }

        public readonly static AlertClassConverter Instance = new AlertClassConverter();
    }
}