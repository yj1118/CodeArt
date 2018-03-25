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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.Radio.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Radio : Input
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register<string, Radio>("Color", () => { return string.Empty; });

        /// <summary>
        /// 输入框的颜色
        /// </summary>
        public string Color
        {
            get
            {
                return (string)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public static readonly DependencyProperty VerticalProperty = DependencyProperty.Register<string, Radio>("Vertical", () => { return false; });

        /// <summary>
        /// 选项是否垂直显示
        /// </summary>
        public bool Vertical
        {
            get
            {
                return (bool)GetValue(VerticalProperty);
            }
            set
            {
                SetValue(VerticalProperty, value);
            }
        }

        /// <summary>
        /// 选项集合
        /// </summary>
        public readonly static DependencyProperty OptionsProperty = DependencyProperty.Register<UIElementCollection, Radio>("Options", () => { return new UIElementCollection(); });

        public UIElementCollection Options
        {
            get
            {
                return GetValue(OptionsProperty) as UIElementCollection;
            }
            set
            {
                SetValue(OptionsProperty, value);
            }
        }

    }
}
