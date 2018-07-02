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
    public class ListColumn : ContentControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register<string, ListColumn>("Title", () => { return string.Empty; });

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

        public static readonly DependencyProperty TextAlignProperty = DependencyProperty.Register<string, ListColumn>("TextAlign", () => { return "center"; });

        public string TextAlign
        {
            get
            {
                return GetValue(TextAlignProperty) as string;
            }
            set
            {
                SetValue(TextAlignProperty, value);
            }
        }

        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register<string, ListColumn>("Width", () => { return string.Empty; });

        public string Width
        {
            get
            {
                return GetValue(WidthProperty) as string;
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }
    }
}
