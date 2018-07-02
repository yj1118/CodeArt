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
    public class FormRow : ContentControl
    {
        protected override void Draw(PageBrush brush)
        {
            if (string.IsNullOrEmpty(this.Class))
                brush.Draw("<div class=\"form-group m-form__group\">");
            else
                brush.DrawFormat("<div class=\"form-group m-form__group {0}\">", this.Class);
            this.Content.Render(brush);
            brush.Draw("</div>");
        }
    }
}
