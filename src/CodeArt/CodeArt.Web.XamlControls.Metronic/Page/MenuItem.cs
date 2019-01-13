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
    public class MenuItem : DependencyObject
    {
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register<string, MenuItem>("Id", () => { return string.Empty; });

        public string Id
        {
            get
            {
                return GetValue(IdProperty) as string;
            }
            set
            {
                SetValue(IdProperty, value);
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register<string, MenuItem>("Text", () => { return string.Empty; });

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

        public static readonly DependencyProperty UrlProperty = DependencyProperty.Register<string, MenuItem>("Url", () => { return string.Empty; });

        public string Url
        {
            get
            {
                return GetValue(UrlProperty) as string;
            }
            set
            {
                SetValue(UrlProperty, value);
            }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register<string, MenuItem>("Icon", () => { return string.Empty; });

        public string Icon
        {
            get
            {
                return GetValue(IconProperty) as string;
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }
    }
}
