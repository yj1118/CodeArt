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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Input.Switch.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Switch : Input
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register<string, Switch>("Color", () => { return string.Empty; });

        /// <summary>
        /// success,warning,info,danger等颜色
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
    }
}
