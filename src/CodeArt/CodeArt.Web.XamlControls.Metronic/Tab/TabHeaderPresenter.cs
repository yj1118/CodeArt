using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class TabHeaderPresenter : FrameworkElement
    {
        protected override void Draw(PageBrush brush)
        {
            var tab = this.GetTemplateParent() as Tab;
            var panels = tab.Content;

            brush.DrawLine("<ul class=\"nav nav-tabs\">");

            for (var i = 0; i < panels.Count; i++)
            {
                var panel = panels[i] as TabPanel;

                if (panel.Selected) brush.Draw("<li class=\"active\">");
                else brush.Draw("<li>");
                brush.DrawFormat("<a href=\"#panel_{0}_{1}\" data-toggle=\"tab\">{2}</a></li>", tab.Id, i, panel.Title);
            }

            brush.DrawLine();
            brush.Draw("</ul>");
        }
    }
}
