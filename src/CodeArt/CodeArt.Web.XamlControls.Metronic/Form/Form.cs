using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Form.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Form : ContentControl
    {
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register<UIElementCollection, Form>("Actions", () => { return new UIElementCollection(); });

        public UIElementCollection Actions
        {
            get
            {
                return GetValue(ActionsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ActionsProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Actions.GetChild(childName);
        }

        static Form()
        {

        }

    }
}
