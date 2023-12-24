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
    public class TabPanel : ContentControl
    {
        public static DependencyProperty TitleProperty { get; private set; }

        public static DependencyProperty SelectedProperty { get; private set; }

        static TabPanel()
        {
            var titleMetadata = new PropertyMetadata(() => { return string.Empty; });
            TitleProperty = DependencyProperty.Register<string, TabPanel>("Title", titleMetadata);

            var selectedMetadata = new PropertyMetadata(() => { return false; });
            SelectedProperty = DependencyProperty.Register<bool, TabPanel>("Selected", selectedMetadata);
        }

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

        public bool Selected
        {
            get
            {
                return (bool)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }
    }
}
