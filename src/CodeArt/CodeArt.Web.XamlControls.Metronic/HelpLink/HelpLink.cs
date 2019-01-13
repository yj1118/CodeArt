using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.XamlControls.Metronic;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Script;
using FormSE = CodeArt.Web.XamlControls.Metronic.FormSE;
using CodeArt.Util;
using CodeArt.Web.WebPages;



namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.HelpLink.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class HelpLink : Control
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register<string, HelpLink>("Label", () => { return string.Empty; });

        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public HelpLink()
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        static HelpLink()
        { }
    }
}