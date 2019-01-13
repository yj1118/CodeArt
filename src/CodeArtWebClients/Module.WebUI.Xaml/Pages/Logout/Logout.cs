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
using CodeArt.ServiceModel;
using FormSE = CodeArt.Web.XamlControls.Metronic.FormSE;

namespace Module.WebUI.Xaml.Pages
{
    [TemplateCode("Template", "Module.WebUI.Xaml.Pages.Logout.Template.html,Module.WebUI.Xaml")]
    [TemplateCodeFactory("Template", typeof(DefaultTemplateCodeFactory))]
    public class Logout : CodeArt.Web.WebPages.Xaml.Controls.Page
    {
        public static readonly DependencyProperty ReturnUrlProperty = DependencyProperty.Register<string, Logout>("ReturnUrl", () => { return "/login.htm"; });
        /// <summary>
        /// 
        /// </summary>
        public string ReturnUrl
        {
            get
            {
                return (string)GetValue(ReturnUrlProperty);
            }
            set
            {
                SetValue(ReturnUrlProperty, value);
            }
        }

        public Logout()
        {
            this.Loaded += OnLoaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void OnLoaded(object sender, object e)
        {
            Principal.Logout();
        }
    }
}
   