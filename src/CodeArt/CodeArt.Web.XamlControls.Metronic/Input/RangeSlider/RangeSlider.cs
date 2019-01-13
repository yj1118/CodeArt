using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.RangeSlider.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class RangeSlider : Input
    {
        //public static readonly DependencyProperty ColorProperty = DependencyProperty.Register<string, RangeSlider>("Color", () => { return string.Empty; });

        ///// <summary>
        ///// success,warning,info,danger等颜色
        ///// </summary>
        //public string Color
        //{
        //    get
        //    {
        //        return (string)GetValue(ColorProperty);
        //    }
        //    set
        //    {
        //        SetValue(ColorProperty, value);
        //    }
        //}

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register<int, RangeSlider>("Min", () => { return 0; });

        /// <summary>
        /// 范围的最小值
        /// </summary>
        public int Min
        {
            get
            {
                return (int)GetValue(MinProperty);
            }
            set
            {
                SetValue(MinProperty, value);
            }
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register<int, RangeSlider>("Max", () => { return 100; });

        /// <summary>
        /// 范围的最大值
        /// </summary>
        public int Max
        {
            get
            {
                return (int)GetValue(MaxProperty);
            }
            set
            {
                SetValue(MaxProperty, value);
            }
        }

        public static readonly DependencyProperty StepProperty = DependencyProperty.Register<float, RangeSlider>("Step", () => { return 1.0F; });

        /// <summary>
        /// 每一格的进度
        /// </summary>
        public float Step
        {
            get
            {
                return (float)GetValue(StepProperty);
            }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register<string, RangeSlider>("Type", () => { return "single"; });

        /// <summary>
        /// single：只能移动一端,double：可以移动两端
        /// </summary>
        public string Type
        {
            get
            {
                return (string)GetValue(TypeProperty);
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }


        public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register<string, RangeSlider>("Prefix", () => { return string.Empty; });

        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix
        {
            get
            {
                return (string)GetValue(PrefixProperty);
            }
            set
            {
                SetValue(PrefixProperty, value);
            }
        }

        public static readonly DependencyProperty PostfixProperty = DependencyProperty.Register<string, RangeSlider>("Postfix", () => { return string.Empty; });

        /// <summary>
        /// 后缀
        /// </summary>
        public string Postfix
        {
            get
            {
                return (string)GetValue(PostfixProperty);
            }
            set
            {
                SetValue(PostfixProperty, value);
            }
        }

    }
}
