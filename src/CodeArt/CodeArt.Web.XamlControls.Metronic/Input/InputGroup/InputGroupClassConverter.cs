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
    public class InputGroupClassConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var baseClass = parameter.ToString();
            var d = RenderContext.Current.BelongTemplate.TemplateParent as Input;
            if(d.Prepend.Count > 0 || d.Append.Count > 0)
            {
                return string.Format("{0} input-group", baseClass);
            }
            return baseClass;
        }

        public readonly static InputGroupClassConverter Instance = new InputGroupClassConverter();
    }
}