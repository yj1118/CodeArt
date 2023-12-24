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
    public class UserSecurityItem : FrameworkElement
    {
        public static DependencyProperty IconProperty { get; private set; }
        public static DependencyProperty IconColorProperty { get; private set; }
        public static DependencyProperty TypeProperty { get; private set; }

        public static DependencyProperty TitleProperty { get; private set; }
        public static DependencyProperty BindedTextProperty { get; private set; }
        public static DependencyProperty UnBindTextProperty { get; private set; }

        public static DependencyProperty IsSetProperty { get; private set; }

        static UserSecurityItem()
        {
            var iconMetadata = new PropertyMetadata(() => { return string.Empty; });
            IconProperty = DependencyProperty.Register<string, UserSecurityItem>("Icon", iconMetadata);

            var iconColorMetadata = new PropertyMetadata(() => { return string.Empty; });
            IconColorProperty = DependencyProperty.Register<string, UserSecurityItem>("IconColor", iconColorMetadata);

            var typeMetadata = new PropertyMetadata(() => { return string.Empty; });
            TypeProperty = DependencyProperty.Register<string, UserSecurityItem>("Type", typeMetadata);

            var titleMetadata = new PropertyMetadata(() => { return string.Empty; });
            TitleProperty = DependencyProperty.Register<string, UserSecurityItem>("Title", titleMetadata);

            var bindedTextMetadata = new PropertyMetadata(() => { return string.Empty; });
            BindedTextProperty = DependencyProperty.Register<string, UserSecurityItem>("BindedText", bindedTextMetadata);

            var unBindTextMetadata = new PropertyMetadata(() => { return string.Empty; });
            UnBindTextProperty = DependencyProperty.Register<string, UserSecurityItem>("UnBindText", unBindTextMetadata);

            var isSetMetadata = new PropertyMetadata(() => { return false; });
            IsSetProperty = DependencyProperty.Register<bool, UserSecurityItem>("IsSet", isSetMetadata);

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

        public string IconColor
        {
            get
            {
                return GetValue(IconColorProperty) as string;
            }
            set
            {
                SetValue(IconColorProperty, value);
            }
        }

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

        public string BindedText
        {
            get
            {
                return GetValue(BindedTextProperty) as string;
            }
            set
            {
                SetValue(BindedTextProperty, value);
            }
        }

        public string UnBindText
        {
            get
            {
                return GetValue(UnBindTextProperty) as string;
            }
            set
            {
                SetValue(UnBindTextProperty, value);
            }
        }

        public bool IsSet
        {
            get
            {
                return (bool)GetValue(IsSetProperty);
            }
            set
            {
                SetValue(IsSetProperty, value);
            }
        }

        protected override void Draw(PageBrush brush)
        {
            brush.Draw("<li>");
            brush.DrawLine();

            brush.Draw("<div class=\"col1\"><div class=\"cont\">");
            brush.DrawLine();

            brush.DrawFormat("<div class=\"cont-col1\"><div class=\"label label-sm {0}\">", this.IconColor);
            brush.DrawLine();

            brush.DrawFormat("<i class=\"fa {0}\"></i>", this.Icon);
            brush.DrawLine();

            brush.Draw("</div></div>");
            brush.DrawLine();

            brush.Draw("<div class=\"cont-col2\"><div class=\"desc\">");
            brush.DrawLine();

            brush.DrawFormat("<em>{0}</em>", this.Title);
            brush.DrawLine();

            if (this.IsSet)
            {
                brush.DrawFormat("<span class=\"label\">{0}</span>", this.BindedText);
                brush.DrawLine();
            }
            else
            {
                brush.DrawFormat("<span class=\"label\">{0}</span>", this.UnBindText);
                brush.DrawLine();
            }

            brush.Draw("</div></div>");
            brush.DrawLine();

            brush.Draw("</div></div>");
            brush.DrawLine();

            if (this.IsSet)
            {
                brush.Draw("<div class=\"col2\">");
                brush.DrawLine();

                brush.Draw("<div class=\"label color-green\"><i class=\"fa fa-check-circle-o\"></i> 已设置</div>");
                brush.DrawLine();

                brush.DrawFormat("<div class=\"label color-default\"><i class=\"fa fa-edit\"></i> <a href=\"javascript: void(0);\" class=\"a-{0}-update\">修改</a></div>", this.Type);
                brush.DrawLine();

                brush.Draw("</div>");
                brush.DrawLine();
            }
            else
            {
                brush.Draw("<div class=\"col2\">");
                brush.DrawLine();

                brush.Draw("<div class=\"label color-red\"><i class=\"fa fa-question-circle\"></i> 未设置</div>");
                brush.DrawLine();

                brush.DrawFormat("<div class=\"label color-default\"><i class=\"fa fa-edit\"></i> <a href=\"javascript: void(0);\" class=\"a-{0}-set\">设置</a></div>", this.Type);
                brush.DrawLine();

                brush.Draw("</div>");
                brush.DrawLine();

            }

            brush.Draw("</li>");
            brush.DrawLine();
        }

    }
}
