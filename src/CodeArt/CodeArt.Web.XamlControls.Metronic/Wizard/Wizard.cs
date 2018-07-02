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
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Wizard.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Wizard : ContentControl
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register<string, Wizard>("Message", () => { return string.Empty; });

        public string Message
        {
            get
            {
                return GetValue(MessageProperty) as string;
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public static readonly DependencyProperty GroupProperty = DependencyProperty.Register<string, Wizard>("Group", () => { return string.Empty; });

        public string Group
        {
            get
            {
                return GetValue(GroupProperty) as string;
            }
            set
            {
                SetValue(GroupProperty, value);
            }
        }

        public readonly static DependencyProperty StepProperty = DependencyProperty.Register<int, Wizard>("Step", () => { return 1; });

        /// <summary>
        /// 步骤
        /// </summary>
        public int Step
        {
            get
            {
                return (int)GetValue(StepProperty);
            }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        public override void OnInit()
        {
            base.OnInit();
            this.Group = Guid.NewGuid().ToString("N");
        }


    }
}
