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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.Select.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Select : Input
    {
        public static readonly DependencyProperty MultipleProperty = DependencyProperty.Register<bool, Select>("Multiple", () => { return false; });

        /// <summary>
        /// 是否可以多选
        /// </summary>
        public bool Multiple
        {
            get
            {
                return (bool)GetValue(MultipleProperty);
            }
            set
            {
                SetValue(MultipleProperty, value);
            }
        }

        public static readonly DependencyProperty MinLengthProperty = DependencyProperty.Register<int, Select>("MinLength", () => { return 0; });

        public int MinLength
        {
            get
            {
                return (int)GetValue(MinLengthProperty);
            }
            set
            {
                SetValue(MinLengthProperty, value);
            }
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register<int, Select>("MaxLength", () => { return 0; });

        public int MaxLength
        {
            get
            {
                return (int)GetValue(MaxLengthProperty);
            }
            set
            {
                SetValue(MaxLengthProperty, value);
            }
        }

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register<string, Select>("Placeholder", () => { return string.Empty; });

        public string Placeholder
        {
            get
            {
                return GetValue(PlaceholderProperty) as string;
            }
            set
            {
                SetValue(PlaceholderProperty, value);
            }
        }

        public static readonly DependencyProperty ModalProperty = DependencyProperty.Register<string, Select>("Modal", () => { return string.Empty; });

        public string Modal
        {
            get
            {
                return GetValue(ModalProperty) as string;
            }
            set
            {
                SetValue(ModalProperty, value);
            }
        }


        /// <summary>
        /// 选项集合
        /// </summary>
        public readonly static DependencyProperty OptionsProperty = DependencyProperty.Register<UIElementCollection, Select>("Options", () => { return new UIElementCollection(); });

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
