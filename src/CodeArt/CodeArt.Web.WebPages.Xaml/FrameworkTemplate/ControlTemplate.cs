using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages;

namespace CodeArt.Web.WebPages.Xaml
{
    public class ControlTemplate : FrameworkTemplate, IControlTemplate
    {
        static ControlTemplate()
        {
        }

        public ControlTemplate()
        {

        }

        protected override void Render(object templateParent, PageBrush brush)
        {
            var e = this.Template;
            if (e == null) return;
            var parent = templateParent as DependencyObject;
            foreach (var item in e)
            {
                var t = item as UIElement;
                if (t != null)
                {
                    t.Parent = parent;
                    t.Render(brush);
                }
                else brush.Draw(t.ToString());
            }
        }
    }
}