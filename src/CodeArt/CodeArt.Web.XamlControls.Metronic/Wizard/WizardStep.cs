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
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class WizardStep : ContentControl
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register<string, WizardStep>("Icon", () => { return "fa flaticon-placeholder"; });

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

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register<string, WizardStep>("Title", () => { return string.Empty; });

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


        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register<string, WizardStep>("Message", () => { return string.Empty; });

        public string Message
        {
            get
            {
                return GetValue(MessageProperty) as string;
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }
    }
}
