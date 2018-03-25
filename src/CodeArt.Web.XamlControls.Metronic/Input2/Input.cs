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
    [ComponentLoaderFactory(typeof(InputLoaderFactory))]
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Input : ContentControl
    {
        public static DependencyProperty PlaceholderProperty { get; private set; }

        public static DependencyProperty TypeProperty { get; private set; }

        public static DependencyProperty LabelProperty { get; private set; }

        public static DependencyProperty BrowseProperty { get; private set; }

        public static DependencyProperty CoreProperty { get; private set; }

        public static DependencyProperty HelpProperty { get; private set; }


        static Input()
        {
            var placeholderMetadata = new PropertyMetadata(() => { return string.Empty; });
            PlaceholderProperty = DependencyProperty.Register<string, Input>("Placeholder", placeholderMetadata);

            var typeMetadata = new PropertyMetadata(() => { return string.Empty; }, OnGotType);
            TypeProperty = DependencyProperty.Register<string, Input>("Type", typeMetadata);

            var labelMetadata = new PropertyMetadata(() => { return null; }, OnGotLabel);
            LabelProperty = DependencyProperty.Register<InputLabel, Input>("Label", labelMetadata);

            var browseMetadata = new PropertyMetadata(() => { return null; }, OnGotBrowse);
            BrowseProperty = DependencyProperty.Register<InputBrowse, Input>("Browse", browseMetadata);

            var helpMetadata = new PropertyMetadata(() => { return null; }, OnGotHelp);
            HelpProperty = DependencyProperty.Register<InputHelp, Input>("Help", helpMetadata);

            var coreMetadata = new PropertyMetadata(() => { return null; }, OnGotCore);
            CoreProperty = DependencyProperty.Register<InputCore, Input>("Core", coreMetadata);
        }

        private static void OnGotType(DependencyObject obj, ref object baseValue)
        {
            (obj as Input).OnGotType(ref baseValue);
        }

        private static T FindPart<T>(DependencyObject obj) where T : class
        {
            var content = (obj as Input).Content;
            foreach (var t in content)
            {
                var target = t as T;
                if (target != null) return target;
            }
            return null;
        }

        private static void OnGotLabel(DependencyObject obj, ref object baseValue)
        {
            baseValue = FindPart<InputLabel>(obj);
        }

        private static void OnGotBrowse(DependencyObject obj, ref object baseValue)
        {
            baseValue = FindPart<InputBrowse>(obj);
        }

        private static void OnGotHelp(DependencyObject obj, ref object baseValue)
        {
            baseValue = FindPart<InputHelp>(obj);
        }

        private static void OnGotCore(DependencyObject obj, ref object baseValue)
        {
            baseValue = FindPart<InputCore>(obj);
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

        protected virtual void OnGotType(ref object baseValue) { }

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

        public InputLabel Label
        {
            get
            {
                return GetValue(LabelProperty) as InputLabel;
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public InputBrowse Browse
        {
            get
            {
                return GetValue(BrowseProperty) as InputBrowse;
            }
            set
            {
                SetValue(BrowseProperty, value);
            }
        }

        public InputHelp Help
        {
            get
            {
                return GetValue(HelpProperty) as InputHelp;
            }
            set
            {
                SetValue(HelpProperty, value);
            }
        }

        public InputCore Core
        {
            get
            {
                return GetValue(CoreProperty) as InputCore;
            }
            set
            {
                SetValue(CoreProperty, value);
            }
        }
    }
}