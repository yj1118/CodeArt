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
    public class SwitchCoreClassConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var d = RenderContext.Current.BelongTemplate.TemplateParent as Switch;
            if (string.IsNullOrEmpty(d.Color)) return "m-switch";
            return string.Format("m-switch m-switch--{0}", d.Color);
        }

        public readonly static SwitchCoreClassConverter Instance = new SwitchCoreClassConverter();
    }
}