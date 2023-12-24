using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.ModuleNest;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class UserProfileItem : FrameworkElement
    {
        public static DependencyProperty TextProperty { get; private set; }
        public static DependencyProperty IconProperty { get; private set; }
        public static DependencyProperty UrlProperty { get; private set; }

        static UserProfileItem()
        {
            var textMetadata = new PropertyMetadata(() => { return string.Empty; });
            TextProperty = DependencyProperty.Register<string, UserProfileItem>("Text", textMetadata);

            var iconMetadata = new PropertyMetadata(() => { return string.Empty; });
            IconProperty = DependencyProperty.Register<string, UserProfileItem>("Icon", iconMetadata);

            var urlMetadata = new PropertyMetadata(() => { return string.Empty; });
            UrlProperty = DependencyProperty.Register<string, UserProfileItem>("Url", urlMetadata);
        }

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

        protected override void Draw(PageBrush brush)
        {
            brush.DrawFormat(string.Format("<li><a href=\"{0}\"><i class=\"{1}\"></i> {2}</a></li>",
                this.Url, this.Icon, this.Text));

            brush.DrawLine();
        }

    }
}
