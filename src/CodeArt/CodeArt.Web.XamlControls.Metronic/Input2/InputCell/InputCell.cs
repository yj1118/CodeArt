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
    public class InputCell : ContentControl
    {
        public static DependencyProperty TypeProperty { get; private set; }

        static InputCell()
        {
            var typeMetadata = new PropertyMetadata(() => { return null; });
            TypeProperty = DependencyProperty.Register<string, InputCore>("Type", typeMetadata);
        }

        public string Type
        {
            get
            {
                return GetValue(TypeProperty) as string;
            }
            set
            {
                SetValue(TypeProperty, value);
            }
        }

        protected override void Draw(PageBrush brush)
        {
            var type = this.Type;

            if (type.Equals("button", StringComparison.OrdinalIgnoreCase))
            {
                brush.Draw("<span class=\"input-group-btn\">");
                this.Content.Render(brush);
                brush.Draw("</span>");
            }
            else if (type.Equals("text", StringComparison.OrdinalIgnoreCase))
            {
                brush.Draw("<span class=\"input-group-addon\">");
                this.Content.Render(brush);
                brush.Draw("</span>");
            }
        }

    }
}
