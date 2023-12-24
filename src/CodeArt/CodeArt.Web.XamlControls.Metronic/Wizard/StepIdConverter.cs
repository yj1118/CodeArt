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
    public class StepIdConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var wizard = RenderContext.Current.BelongTemplate.TemplateParent as Wizard;
            var group = wizard.Group;
            var index = parameter.ToString();
            return string.Format("wizard_{0}_{1}", group, index);
        }

        public readonly static StepIdConverter Instance = new StepIdConverter();
    }
}