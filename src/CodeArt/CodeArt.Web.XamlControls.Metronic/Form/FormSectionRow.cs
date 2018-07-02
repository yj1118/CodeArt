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
    public class FormSectionRow : ContentControl
    {
        protected override void Draw(PageBrush brush)
        {
            brush.DrawLine("<div class=\"form-group m-form__group row m--padding-top-10 m--padding-bottom-10\">");
            //根据内容数量，自动分列
            var content = this.Content;
            if (content.Count > 0)
            {
                var col = 12 / content.Count;
                foreach (UIElement child in content)
                {
                    brush.DrawFormat("<div class=\"col-lg-{0}\">", col);
                    brush.DrawLine();
                    child.Render(brush);
                    brush.DrawLine("</div>");
                }
            }
            brush.DrawLine("</div>");
        }
    }
}
