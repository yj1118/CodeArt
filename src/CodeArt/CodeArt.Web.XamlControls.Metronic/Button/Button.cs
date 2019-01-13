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
    public class Button : Control
    {
        public static readonly DependencyProperty TextProperty  = DependencyProperty.Register<string, Button>("Text", () => { return string.Empty; });

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

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register<string, Button>("Icon", () => { return string.Empty; });

        /// <summary>
        /// 图标
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

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register<string, Button>("Color", () => { return string.Empty; });

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

        public static readonly DependencyProperty HoverProperty = DependencyProperty.Register<string, Button>("Hover", () => { return string.Empty; });

        /// <summary>
        /// 鼠标移上去的颜色的名称，注意是类似success,danger等名称，而不是十六进制的颜色值
        /// </summary>
        public string Hover
        {
            get
            {
                return GetValue(HoverProperty) as string;
            }
            set
            {
                SetValue(HoverProperty, value);
            }
        }

        public static readonly DependencyProperty OutlineProperty = DependencyProperty.Register<string, Button>("Outline", () => { return 0; });

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

        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register<string, Button>("Shape", () => { return string.Empty; });

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

        public static readonly DependencyProperty AirProperty = DependencyProperty.Register<string, Button>("Air", () => { return false; });

        /// <summary>
        /// 是否悬浮，让按钮看起来更有立体感
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

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register<string, Button>("Size", () => { return string.Empty; });

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

        /// <summary>
        /// 仅显示图标
        /// </summary>
        public bool IconOnly
        {
            get
            {
                return !string.IsNullOrEmpty(this.Icon) && string.IsNullOrEmpty(this.Text);
            }
        }

        public bool TextOnly
        {
            get
            {
                return string.IsNullOrEmpty(this.Icon) && !string.IsNullOrEmpty(this.Text);
            }
        }


        public static readonly DependencyProperty HrefProperty = DependencyProperty.Register<string, Button>("Href", () => { return string.Empty; });

        /// <summary>
        /// 文本
        /// </summary>
        public string Href
        {
            get
            {
                return GetValue(HrefProperty) as string;
            }
            set
            {
                SetValue(HrefProperty, value);
            }
        }

        public Button()
        {

        }

        protected override void Draw(PageBrush brush)
        {
            DrawBegin(brush);
            if(this.IconOnly)
            {
                brush.DrawFormat("<i class='{0}'></i>", this.Icon);
                brush.DrawLine();
            }
            else if(this.TextOnly)
            {
                brush.DrawFormat("<span>{0}</span>", this.Text);
                brush.DrawLine();
            }
            else
            {
                brush.DrawFormat("<span><i class=\"{0}\"></i><span>{1}</span></span>", this.Icon, this.Text);
                brush.DrawLine();
            }
            DrawEnd(brush);
        }

        private void DrawBegin(PageBrush brush)
        {
            if (string.IsNullOrEmpty(this.Href))
            {
                brush.Draw("<a href='javascript:;'");
            }
            else
            {
                brush.DrawFormat("<a href='{0}'", this.Href);
            }

            if (!string.IsNullOrEmpty(this.Id))
            {
                brush.DrawFormat(" id=\"{0}\"", this.Id);
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                brush.DrawFormat(" name=\"{0}\"", this.Name);
            }


            if (!string.IsNullOrEmpty(this.Color))
            {
                if (this.Outline > 0)
                {
                    brush.DrawFormat(" class='btn btn-outline-{0} m-btn", this.Color);
                }
                else
                {
                    brush.DrawFormat(" class='btn btn-{0} m-btn", this.Color);
                }
            }
            else brush.Draw(" class='btn m-btn");

            if (!string.IsNullOrEmpty(this.Hover))
            {
                brush.DrawFormat(" m-btn--hover-{0}", this.Hover);
            }

            if (!string.IsNullOrEmpty(this.Icon))
            {
                brush.Draw(" m-btn--icon");
                if (string.IsNullOrEmpty(this.Text))
                    brush.Draw(" m-btn--icon-only");
            }

            if (!string.IsNullOrEmpty(this.Size))
            {
                if (this.Size.EqualsIgnoreCase("custom"))
                    brush.Draw(" m-btn--custom");
                else
                    brush.DrawFormat(" btn-{0}", this.Size);
            }

            if (this.Outline > 1)
            {
                brush.DrawFormat(" m-btn--outline-{0}x", this.Outline);
            }

            if (!string.IsNullOrEmpty(this.Shape))
            {
                brush.DrawFormat(" m-btn--{0}", this.Shape);
            }

            if (this.Air)
            {
                brush.Draw(" m-btn--air");
            }

            if(!string.IsNullOrEmpty(this.Class))
            {
                brush.DrawFormat(" {0}", this.Class);
            }


            brush.Draw("' "); //class输出完毕

            XamlUtil.OutputAttributes(this.Attributes, brush);

            if(!string.IsNullOrEmpty(this.ProxyCode))
            {
                brush.DrawFormat(" data-proxy=\"{0}\" ",this.ProxyCode);
            }

            XamlUtil.OutputAttributes(this.Attributes, brush);

            brush.DrawLine(">");
        }

        private void DrawEnd(PageBrush brush)
        {
            brush.Draw("</a>");
        }
    }
}
