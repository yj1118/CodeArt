using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.DTO;
using CodeArt.ModuleNest;

using CodeArt.Web.WebPages;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class MenuSection : DependencyObject
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register<string, MenuSection>("Text", () => { return string.Empty; });

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
