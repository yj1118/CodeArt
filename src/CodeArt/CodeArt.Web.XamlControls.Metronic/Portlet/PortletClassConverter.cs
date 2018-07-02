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
    public class PortletClassConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var portlet = RenderContext.Current.BelongTemplate.TemplateParent as Portlet;
            var baseClass = portlet.GetClass();
            return string.Format("{0} {1}", baseClass, value);

        }

        public readonly static PortletClassConverter Instance = new PortletClassConverter();
    }
}