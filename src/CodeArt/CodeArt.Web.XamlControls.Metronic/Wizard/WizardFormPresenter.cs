using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class WizardFormPresenter : FrameworkElement
    {
        protected override void Draw(PageBrush brush)
        {
            var wizard = this.BelongTemplate.TemplateParent as Wizard;
            var content = wizard.Content;
            var index = 0;
            foreach(WizardStep step in content)
            {
                brush.DrawFormat("<div class=\"m-wizard__form-step {0}\" id=\"wizard_{1}_{2}\">",
                    index == 0 ? "m-wizard__form-step--current" : string.Empty, wizard.Group, index);
                step.Content.Render(brush);
                brush.DrawLine("</div>");
                index++;
            }
        }
    }
}
