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
    public class Option : Control
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register<string, Option>("Value", () => { return string.Empty; });

        public string Value
        {
            get
            {
                return GetValue(ValueProperty) as string;
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register<string, Option>("Text", () => { return string.Empty; });

        public string Text
        {
            get
            {
                return GetValue(TextProperty) as string;
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

    }
}
