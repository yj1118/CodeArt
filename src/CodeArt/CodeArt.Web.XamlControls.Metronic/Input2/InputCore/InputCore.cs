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
    [ComponentLoaderFactory(typeof(InputCoreLoaderFactory))]
    [TemplateCodeFactory("Template",typeof(InputCoreTemplateCodeFactory))]
    public class InputCore : Control
    {
        public static DependencyProperty TypeProperty { get; private set; }

        public static DependencyProperty AlignProperty { get; private set; }

        public static DependencyProperty BeforeProperty { get; private set; }

        public static DependencyProperty AfterProperty { get; private set; }

        public static DependencyProperty PlaceholderProperty { get; private set; }


        static InputCore()
        {
            var typeMetadata = new PropertyMetadata(() => { return null; },OnGotType);
            TypeProperty = DependencyProperty.Register<string, InputCore>("Type", typeMetadata);

            var placeholderMetadata = new PropertyMetadata(() => { return null; }, OnGotPlaceholder);
            PlaceholderProperty = DependencyProperty.Register<string, InputCore>("Placeholder", placeholderMetadata);

            var alignMetadata = new PropertyMetadata(() => { return "left"; });
            AlignProperty = DependencyProperty.Register<string, InputCore>("Align", alignMetadata);

            var beforeMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            BeforeProperty = DependencyProperty.Register<UIElementCollection, InputCore>("Before", beforeMetadata);

            var afterMetadata = new PropertyMetadata(() => { return new UIElementCollection(); });
            AfterProperty = DependencyProperty.Register<UIElementCollection, InputCore>("After", afterMetadata);
        }

        private static void OnGotType(DependencyObject obj, ref object baseValue)
        {
            var input = (obj as InputCore).Parent as Input;
            if (input == null) throw new XamlException("组件 " + typeof(InputCore).FullName + " 必须作为 " + typeof(Input).FullName + " 的直系组件");
            baseValue = input.Type;
        }

        private static void OnGotPlaceholder(DependencyObject obj, ref object baseValue)
        {
            var input = (obj as InputCore).Parent as Input;
            if (input == null) return;
            baseValue = input.Placeholder;
        }

        public string Type
        {
            get
            {
                return GetValue(TypeProperty) as string;
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        public string Align
        {
            get
            {
                return GetValue(AlignProperty) as string;
            }
            set
            {
                SetValue(AlignProperty, value);
            }
        }

        public UIElementCollection Before
        {
            get
            {
                return GetValue(BeforeProperty) as UIElementCollection;
            }
            set
            {
                SetValue(BeforeProperty, value);
            }
        }

        public UIElementCollection After
        {
            get
            {
                return GetValue(AfterProperty) as UIElementCollection;
            }
            set
            {
                SetValue(AfterProperty, value);
            }
        }

        public string Placeholder
        {
            get
            {
                return GetValue(PlaceholderProperty) as string;
            }
            set
            {
                SetValue(PlaceholderProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Before.GetChild(childName) ?? this.After.GetChild(childName);
        }

    }
}
