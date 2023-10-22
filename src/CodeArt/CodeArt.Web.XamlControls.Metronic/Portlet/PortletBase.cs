using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class PortletBase : ContentControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register<string, PortletBase>("Title", () => { return string.Empty; });
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register<string, PortletBase>("Color", () => { return string.Empty; });
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

        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register<string, PortletBase>("BackgroundColor", () => { return "#f2f3f8"; });

        public string BackgroundColor
        {
            get
            {
                return (string)GetValue(BackgroundColorProperty);
            }
            set
            {
                SetValue(BackgroundColorProperty, value);
            }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register<UIElementCollection, PortletBase>("Description", () => { return new UIElementCollection(); });
        public UIElementCollection Description
        {
            get
            {
                return GetValue(DescriptionProperty) as UIElementCollection;
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        public readonly static DependencyProperty ToolsProperty = DependencyProperty.Register<UIElementCollection, PortletBase>("Tools", () => { return new UIElementCollection(); });

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

        public static readonly DependencyProperty CanToggleProperty = DependencyProperty.Register<bool, PortletBase>("CanToggle", () => { return false; });
        public bool CanToggle
        {
            get
            {
                return (bool)GetValue(CanToggleProperty);
            }
            set
            {
                SetValue(CanToggleProperty, value);
            }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register<string, PortletBase>("Icon", () => { return "flaticon-statistics"; });

        public string Icon
        {
            get
            {
                return (string)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public static readonly DependencyProperty BodyClassProperty = DependencyProperty.Register<string, PortletBase>("BodyClass", () => { return string.Empty; });
        public string BodyClass
        {
            get
            {
                return (string)GetValue(BodyClassProperty);
            }
            set
            {
                SetValue(BodyClassProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Tools.GetChild(childName) ?? this.Description.GetChild(childName);
        }

        public override IEnumerable<UIElement> GetActionElement(string actionName)
        {
            return this.Combine(base.GetActionElement(actionName) , this.Tools.GetActionElement(actionName) , this.Description.GetActionElement(actionName));
        }

        public PortletBase()
        {
        }


        static PortletBase()
        { }
    }
}