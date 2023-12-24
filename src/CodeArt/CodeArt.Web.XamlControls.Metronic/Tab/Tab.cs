using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Tab.Template.html,CodeArt.Web.XamlControls.Metronic")]
    [ComponentLoader(typeof(TabLoader))]
    public class Tab : ContentControl
    {
        public static DependencyProperty PanelsProperty { get; private set; }

        static Tab()
        {
            var panelsMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            PanelsProperty = DependencyProperty.Register<UIElementCollection, Tab>("Panels", panelsMetadata);
        }

        public UIElementCollection Panels
        {
            get
            {
                return GetValue(PanelsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(PanelsProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Panels.GetChild(childName);
        }
    }
}
