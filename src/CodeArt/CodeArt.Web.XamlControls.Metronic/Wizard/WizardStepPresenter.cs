using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class WizardStepPresenter : FrameworkElement
    {
        protected override void Draw(PageBrush brush)
        {
            var wizard = this.BelongTemplate.TemplateParent as Wizard;
            var content = wizard.Content;
            var index = 0;
            foreach(WizardStep step in content)
            {
                brush.DrawFormat("<div class=\"m-wizard__step {0}\" data-wizard-target=\"#wizard_{1}_{2}\">",
                    index == 0 ? "m-wizard__step--current" : string.Empty, wizard.Group, index);
                brush.DrawLine();
                brush.DrawFormat("<a href=\"javascript:undefined;\" class=\"m-wizard__step-number\"><span><i class=\"{0}\"></i></span></a>", step.Icon);
                brush.DrawLine();
                brush.DrawLine("<div class=\"m-wizard__step-info\">");
                brush.DrawFormat("<div class=\"m-wizard__step-title\">{0}</div>", step.Title);
                brush.DrawLine();
                brush.DrawFormat("<div class=\"m-wizard__step-desc\">{0}</div>", step.Message);
                brush.DrawLine();
                brush.DrawLine("</div>");
                brush.DrawLine("</div>");
                index++;
            }
        }
    }
}
