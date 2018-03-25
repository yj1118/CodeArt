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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Alert.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Alert : ContentControl
    {
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register<string, Alert>("Shape", () => { return string.Empty; });

        /// <summary>
        /// 形状，有2个值：空（默认的形状）,square(直角矩形)
        /// </summary>
        public string Shape
        {
            get
            {
                return GetValue(ShapeProperty) as string;
            }
            set
            {
                SetValue(ShapeProperty, value);
            }
        }


        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register<string, Alert>("Color", () => { return string.Empty; });

        /// <summary>
        /// 颜色的名称，注意是类似success,danger等名称，而不是十六进制的颜色值
        /// </summary>
        public string Color
        {
            get
            {
                return GetValue(ColorProperty) as string;
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public static readonly DependencyProperty OutlineProperty = DependencyProperty.Register<string, Alert>("Outline", () => { return 0; });

        /// <summary>
        /// 轮廓的粗细，0表示没有轮廓，为1或者2表示轮廓的粗细
        /// </summary>
        public int Outline
        {
            get
            {
                return (int)GetValue(OutlineProperty);
            }
            set
            {
                SetValue(OutlineProperty, value);
            }
        }


        public static readonly DependencyProperty AirProperty = DependencyProperty.Register<string, Alert>("Air", () => { return false; });

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


        public static readonly DependencyProperty IconProperty = DependencyProperty.Register<string, Alert>("Icon", () => { return string.Empty; });

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

        public static readonly DependencyProperty CloseProperty = DependencyProperty.Register<bool, Alert>("Close", () => { return false; });

        /// <summary>
        /// 是否可以关闭
        /// </summary>
        public bool Close
        {
            get
            {
                return (bool)GetValue(CloseProperty);
            }
            set
            {
                SetValue(CloseProperty, value);
            }
        }


        public static readonly DependencyProperty HideProperty = DependencyProperty.Register<bool, Alert>("Hide", () => { return false; });

        /// <summary>
        /// 是否隐藏该alert
        /// </summary>
        public bool Hide
        {
            get
            {
                return (bool)GetValue(HideProperty);
            }
            set
            {
                SetValue(HideProperty, value);
            }
        }



        internal string GetClass()
        {
            using (var temp = StringPool.Borrow())
            {
                var sb = temp.Item;
                sb.Append("alert m-alert");
                if(this.Outline ==1)
                {
                    sb.Append(" m-alert--outline");
                }
                else if(this.Outline == 2)
                {
                    sb.Append(" m-alert--outline m-alert--outline-2x");
                }

                if (this.Air) sb.Append(" m-alert--air");

                if (!string.IsNullOrEmpty(this.Shape)) sb.Append(" m-alert--square");

                if (!string.IsNullOrEmpty(this.Color))
                {
                    if(this.Color == "default")
                        sb.Append(" m-alert--default");
                    else
                        sb.AppendFormat(" alert-{0}", this.Color);
                }

                if (!string.IsNullOrEmpty(this.Icon)) sb.Append(" m-alert--icon");

                if (this.Close) sb.Append(" alert-dismissible fade show");

                if (this.Hide) sb.Append(" m--hide");

                return sb.ToString();
            }
        }

    }
}
