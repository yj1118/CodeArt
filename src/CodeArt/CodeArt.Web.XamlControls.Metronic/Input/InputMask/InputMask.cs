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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.InputMask.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class InputMask : Input
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register<string, InputMask>("Placeholder", () => { return string.Empty; });

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

        /// <summary>
        /// 格式化输入的类型，number代表整型
        /// </summary>
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register<string, InputMask>("Type", () => { return "number"; });

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


    }
}
