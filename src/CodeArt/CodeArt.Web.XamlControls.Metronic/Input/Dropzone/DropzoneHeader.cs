using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class DropzoneHeader : Control
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register<string, DropzoneHeader>("Key", () => { return string.Empty; });

        public string Key
        {
            get
            {
                return GetValue(KeyProperty) as string;
            }
            set
            {
                SetValue(KeyProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register<string, DropzoneHeader>("Value", () => { return string.Empty; });

        public string Value
        {
            get
            {
                return GetValue(ValueProperty).ToString();
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
    }
}
