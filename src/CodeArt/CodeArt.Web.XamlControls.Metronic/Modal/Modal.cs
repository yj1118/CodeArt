using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Modal.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Modal : ContentControl
    {
        public readonly static DependencyProperty FooterProperty = DependencyProperty.Register<UIElementCollection, Modal>("Footer", () => { return new UIElementCollection(); });

        public readonly static DependencyProperty SizeProperty = DependencyProperty.Register<bool, Modal>("Size", ()=> { return string.Empty; });

        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register<bool, Modal>("Title", () => { return string.Empty; });

        public readonly static DependencyProperty BodyHeightProperty = DependencyProperty.Register<double, Modal>("BodyHeight", () => { return (double)0; });

        public readonly static DependencyProperty CenterProperty = DependencyProperty.Register<bool, Modal>("Center", () => { return true; });

        public UIElementCollection Footer
        {
            get
            {
                return GetValue(FooterProperty) as UIElementCollection;
            }
            set
            {
                SetValue(FooterProperty, value);
            }
        }

        public string Size
        {
            get
            {
                return (string)GetValue(SizeProperty);
            }
            set
            {
                SetValue(SizeProperty, value);
            }
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

        /// <summary>
        /// 主体高度，如果设置了高度，那么内容超出高度时会有滚动条
        /// </summary>
        public double BodyHeight
        {
            get
            {
                return (double)GetValue(BodyHeightProperty);
            }
            set
            {
                SetValue(BodyHeightProperty, value);
            }
        }

        /// <summary>
        /// 窗口是否垂直居中
        /// </summary>
        public bool Center
        {
            get
            {
                return (bool)GetValue(CenterProperty);
            }
            set
            {
                SetValue(CenterProperty, value);
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Footer.GetChild(childName);
        }

    }
}
