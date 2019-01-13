using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class Loading : Control
    {
        public static readonly DependencyProperty TextProperty  = DependencyProperty.Register<string, Loading>("Text", () => { return string.Empty; });

        /// <summary>
        /// 文本
        /// </summary>
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

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register<string, Loading>("Color", () => { return "primary"; });

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


        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register<string, Loading>("Shape", () => { return string.Empty; });

        /// <summary>
        /// 形状，有3个值：空（默认的形状）,square(直角矩形),pill（圆形）
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

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register<string, Loading>("Size", () => { return "lg"; });

        /// <summary>
        /// 大小，有4个值：空（默认大小）,sm,lg,custom
        /// </summary>
        public string Size
        {
            get
            {
                return GetValue(SizeProperty) as string;
            }
            set
            {
                SetValue(SizeProperty, value);
            }
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register<string, Loading>("Position", () => { return "center"; });

        /// <summary>
        /// 位置，默认居中
        /// </summary>
        public string Position
        {
            get
            {
                return GetValue(PositionProperty) as string;
            }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        public Loading()
        {

        }

        protected override void Draw(PageBrush brush)
        {
            brush.DrawFormat("<div class=\"m-loader m-loader--{0} m-loader--{1} c-loader--{2}\"></div>", this.Color, this.Size, this.Position);
        }
    }
}
