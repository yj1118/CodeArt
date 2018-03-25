using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.DTO;
using CodeArt.ModuleNest;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Portlet.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Portlet : ContentControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register<string, Portlet>("Title", () => { return string.Empty; });

        public static readonly DependencyProperty TitleClassProperty = DependencyProperty.Register<string, Portlet>("TitleClass", () => { return string.Empty; });

        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register<string, Portlet>("Subtitle", () => { return string.Empty; });

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register<string, Portlet>("Icon", () => { return string.Empty; });

        public readonly static DependencyProperty ToolsProperty = DependencyProperty.Register<UIElementCollection, Portlet>("Tools", () => { return new UIElementCollection(); });

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

        public string TitleClass
        {
            get
            {
                return GetValue(TitleClassProperty) as string;
            }
            set
            {
                SetValue(TitleClassProperty, value);
            }
        }


        public string Subtitle
        {
            get
            {
                return GetValue(SubtitleProperty) as string;
            }
            set
            {
                SetValue(SubtitleProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        public UIElementCollection Tools
        {
            get
            {
                return GetValue(ToolsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(ToolsProperty, value);
            }
        }

        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register<UIElementCollection, Portlet>("Footer", () => { return new UIElementCollection(); });

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

        public static readonly DependencyProperty AirProperty = DependencyProperty.Register<string, Portlet>("Air", () => { return true; });

        /// <summary>
        /// 是否悬浮，看起来更有立体感
        /// </summary>
        public bool Air
        {
            get
            {
                return (bool)GetValue(AirProperty);
            }
            set
            {
                SetValue(AirProperty, value);
            }
        }

        public static readonly DependencyProperty BorderProperty = DependencyProperty.Register<bool, Portlet>("Border", () => { return false; });

        /// <summary>
        /// 是否带边框
        /// </summary>
        public bool Border
        {
            get
            {
                return (bool)GetValue(BorderProperty);
            }
            set
            {
                SetValue(BorderProperty, value);
            }
        }

        public static readonly DependencyProperty SemiProperty = DependencyProperty.Register<bool, Portlet>("Semi", () => { return false; });

        /// <summary>
        /// 移除标题下的横线
        /// </summary>
        public bool Semi
        {
            get
            {
                return (bool)GetValue(SemiProperty);
            }
            set
            {
                SetValue(SemiProperty, value);
            }
        }

        public static readonly DependencyProperty RoundedProperty = DependencyProperty.Register<bool, Portlet>("Rounded", () => { return false; });

        /// <summary>
        /// 圆角
        /// </summary>
        public bool Rounded
        {
            get
            {
                return (bool)GetValue(RoundedProperty);
            }
            set
            {
                SetValue(RoundedProperty, value);
            }
        }

        public static readonly DependencyProperty SmallHeadProperty = DependencyProperty.Register<bool, Portlet>("SmallHead", () => { return false; });

        /// <summary>
        /// 头部标题区域小一点
        /// </summary>
        public bool SmallHead
        {
            get
            {
                return (bool)GetValue(SmallHeadProperty);
            }
            set
            {
                SetValue(SmallHeadProperty, value);
            }
        }

        public static readonly DependencyProperty BgColorProperty = DependencyProperty.Register<string, Portlet>("BgColor", () => { return string.Empty; });


        public string BgColor
        {
            get
            {
                return GetValue(BgColorProperty) as string;
            }
            set
            {
                SetValue(BgColorProperty, value);
            }
        }

        public Portlet()
        {

        }

        internal string GetClass()
        {
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.Append("m-portlet");

                if (this.Border) sb.Append(" m-portlet--bordered");
                if (this.Semi) sb.Append(" m-portlet--bordered-semi");
                if (this.Rounded) sb.Append(" m-portlet--rounded");
                if (!this.Air) sb.Append(" m-portlet--unair");
                if (this.SmallHead) sb.Append(" m-portlet--head-sm");
                if (!string.IsNullOrEmpty(this.BgColor)) sb.AppendFormat(" m--bg-{0}", this.BgColor);

                return sb.ToString();
            }
        }

        public override DependencyObject GetChild(string childName)
        {
            return base.GetChild(childName) ?? this.Tools.GetChild(childName) ?? this.Footer.GetChild(childName);
        }

    }
}
