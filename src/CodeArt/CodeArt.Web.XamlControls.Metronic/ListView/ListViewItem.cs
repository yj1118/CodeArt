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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.ListView.ListViewItem.html,CodeArt.Web.XamlControls.Metronic")]
    public class ListViewItem : ContentControl
    {
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register<string, ListViewItem>("Title", () => { return string.Empty; });

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

        public readonly static DependencyProperty TitleFieldProperty = DependencyProperty.Register<string, ListViewItem>("TitleField", () => { return string.Empty; });

        public string TitleField
        {
            get
            {
                return GetValue(TitleFieldProperty) as string;
            }
            set
            {
                SetValue(TitleFieldProperty, value);
            }
        }

        public readonly static DependencyProperty ActionProperty = DependencyProperty.Register<string, ListViewItem>("Action", () => { return string.Empty; });

        public string Action
        {
            get
            {
                return GetValue(ActionProperty) as string;
            }
            set
            {
                SetValue(ActionProperty, value);
            }
        }

        public readonly static DependencyProperty HeaderProperty = DependencyProperty.Register<UIElementCollection, ListViewItem>("Header", () => { return new UIElementCollection(); });

        public UIElementCollection Header
        {
            get
            {
                return GetValue(HeaderProperty) as UIElementCollection;
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Header.GetChild(childName);
        }

        public override IEnumerable<UIElement> GetActionElement(string actionName)
        {
            return this.Combine(base.GetActionElement(actionName), this.Header.GetActionElement(actionName));
        }
    }
}
