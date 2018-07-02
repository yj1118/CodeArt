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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.TextBox.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class TextBox : Input
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register<string, TextBox>("Placeholder", () => { return string.Empty; });

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

        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register<int, TextBox>("Rows", () => { return 1; });

        public int Rows
        {
            get
            {
                return (int)GetValue(RowsProperty);
            }
            set
            {
                SetValue(RowsProperty, value);
            }
        }


        public static readonly DependencyProperty MinLengthProperty = DependencyProperty.Register<int, TextBox>("MinLength", () => { return 0; });

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

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register<int, TextBox>("MaxLength", () => { return 0; });

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

        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register<string, TextBox>("Type", () => { return "text"; });

        /// <summary>
        /// 输入类型：text,password,email
        /// </summary>
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

        public static readonly DependencyProperty EqualToPrevProperty = DependencyProperty.Register<string, TextBox>("EqualToPrev", () => { return string.Empty; });

        /// <summary>
        /// 当与上1个元素不同值的时候的错误提示
        /// </summary>
        public string EqualToPrev
        {
            get
            {
                return GetValue(EqualToPrevProperty) as string;
            }
            set
            {
                SetValue(EqualToPrevProperty, value);
            }
        }

        static TextBox()
        {

        }

    }
}
