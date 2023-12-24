using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.ListView.ListViewItemPortlet.html,CodeArt.Web.XamlControls.Metronic")]
    public class ListViewItemPortlet : ContentControl
    {
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register<string, ListViewItemPortlet>("Title", () => { return string.Empty; });

        public string Title
        {
            get
            {
                return GetValue(TitleProperty) as string;
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public readonly static DependencyProperty DescriptionProperty = DependencyProperty.Register<string, ListViewItemPortlet>("Description", () => { return string.Empty; });

        public string Description
        {
            get
            {
                return GetValue(DescriptionProperty) as string;
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register<string, ListViewItemPortlet>("Color", () => { return string.Empty; });
        public string Color
        {
            get
            {
                return (string)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public readonly static DependencyProperty ToolsProperty = DependencyProperty.Register<UIElementCollection, ListViewItemPortlet>("Tools", () => { return new UIElementCollection(); });

        public UIElementCollection Tools
        {
            get
            {
                return GetValue(ToolsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ToolsProperty, value);
            }
        }

        public ListViewItemPortlet()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        static ListViewItemPortlet()
        { }

    }
}
