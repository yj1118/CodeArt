using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml.Controls
{
    public class PC : ContentControl
    {
        public override void OnLoad()
        {
            if (WebPageContext.Current.IsMobileDevice) return;
            base.OnLoad();
        }

        protected override void Draw(PageBrush brush)
        {
            if (WebPageContext.Current.IsMobileDevice) return;
            base.Draw(brush);
        }
    }
}
