using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;
using CodeArt.Web.WebPages;

namespace CodeArt.Web.XamlControls.Metronic
{
    [ContentProperty("Options")]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.UserSecurity.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class UserSecurity : Control
    {
        public static DependencyProperty LevelProperty { get; private set; }
        public static DependencyProperty OptionsProperty { get; private set; }
        public static DependencyProperty AccountIdProperty { get; private set; }

        static UserSecurity()
        {
            var levelMetadata = new PropertyMetadata(() => { return (float)0.0; });
            LevelProperty = DependencyProperty.Register<float, UserSecurity>("Level", levelMetadata);

            var optionsMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            OptionsProperty = DependencyProperty.Register<UIElementCollection, UserSecurity>("Options", optionsMetadata);

            var accountIdMetadata = new PropertyMetadata(() => { return string.Empty; });
            AccountIdProperty = DependencyProperty.Register<string, UserSecurity>("AccountId", accountIdMetadata);

        }

        public float Level
        {
            get
            {
                return (float)GetValue(LevelProperty);
            }
            set
            {
                SetValue(LevelProperty, value);
            }
        }

        public string AccountId
        {
            get
            {
                return GetValue(AccountIdProperty) as string;
            }
            set
            {
                SetValue(AccountIdProperty, value);
            }
        }

        public UIElementCollection Options
        {
            get
            {
                return GetValue(OptionsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(OptionsProperty, value);
            }
        }

        public UserSecurity()
        {
        }
    }
}
