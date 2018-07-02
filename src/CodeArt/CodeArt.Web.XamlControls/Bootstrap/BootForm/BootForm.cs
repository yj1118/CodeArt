using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.XamlControls
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Bootstrap.BootForm.Template.html,CodeArt.Web.XamlControls")]
    [ComponentLoader(typeof(BootFormLoader))]
    public class BootForm : ContentControl
    {
        public static DependencyProperty CommandProperty { get; private set; }

        static BootForm()
        {
            var commandMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            CommandProperty = DependencyProperty.Register<UIElementCollection, BootForm>("Command", commandMetadata);
        }

        public UIElementCollection Command
        {
            get
            {
                return GetValue(CommandProperty) as UIElementCollection;
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Command.GetChild(childName);
        }

        public override void OnLoad()
        {
            this.Content.OnLoad();
            base.OnLoad();
        }
    }
}
