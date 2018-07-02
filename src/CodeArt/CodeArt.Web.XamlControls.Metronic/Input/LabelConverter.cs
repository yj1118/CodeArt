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
    public class LabelConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var d = RenderContext.Current.BelongTemplate.TemplateParent as Input;
            if (d.Required) return string.Format("{0} <span class='m--font-danger'>*</span>", d.Label);
            return d.Label;
        }

        public readonly static LabelConverter Instance = new LabelConverter();
    }
}