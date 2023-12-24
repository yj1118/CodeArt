using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Sealed;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class TabContentPresenter : FrameworkElement
    {
        protected override void Draw(PageBrush brush)
        {
            var tab = this.GetTemplateParent() as Tab;
            var panels = tab.Content;

            brush.DrawLine("<div class=\"tab-content\">");

            for (var i = 0; i < panels.Count; i++)
            {
                var panel = panels[i] as TabPanel;

                if (panel.Selected) brush.Draw("<div class=\"tab-pane fade active in\" ");
                else brush.Draw("<div class=\"tab-pane fade\" ");
                brush.DrawFormat("id=\"panel_{0}_{1}\">", tab.Id, i);

                foreach (UIElement c in panel.Content)
                {
                    c.Render(brush);
                }

                brush.Draw("</div>");
            }

            brush.DrawLine();
            brush.Draw("</div>");
        }
    }
}
