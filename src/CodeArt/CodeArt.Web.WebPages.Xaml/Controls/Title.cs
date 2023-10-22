using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    public class Title : ContentControl
    {
        public Title()
        {
        }

        protected override void Draw(PageBrush brush)
        {
            brush.Draw("<title>", DrawOrigin.Current);
            this.Content.Render(brush);
            brush.Draw("</title>", DrawOrigin.Current);
        }
    }
}
