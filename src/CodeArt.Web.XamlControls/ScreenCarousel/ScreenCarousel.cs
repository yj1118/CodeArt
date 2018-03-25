using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.XamlControls
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.ScreenCarousel.Template.html,CodeArt.Web.XamlControls")]
    [TemplateCode("ItemTemplate", "CodeArt.Web.XamlControls.ScreenCarousel.ItemTemplate.html,CodeArt.Web.XamlControls")]
    public class ScreenCarousel : ItemsControl
    {

        public static DependencyProperty SkinsPathProperty { get; private set; }

        static ScreenCarousel()
        {
            var skinsPathMetadata = new PropertyMetadata(() => { return string.Empty; });
            SkinsPathProperty = DependencyProperty.Register<string, ScreenCarousel>("SkinsPath", skinsPathMetadata);
        }

        public string SkinsPath
        {
            get
            {
                return GetValue(SkinsPathProperty) as string;
            }
            set
            {
                SetValue(SkinsPathProperty, value);
            }
        }
    }
}
