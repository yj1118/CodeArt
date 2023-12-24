using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;
using CodeArt.Util;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class Label : ContentControl
    {
        public static readonly DependencyProperty AlignProperty = DependencyProperty.Register<string, Label>("Align", () => { return "right"; });

        /// <summary>
        /// 
        /// </summary>
        public string Align
        {
            get
            {
                return GetValue(AlignProperty) as string;
            }
            set
            {
                SetValue(AlignProperty, value);
            }
        }


        public Label()
        {

        }

        protected override void Draw(PageBrush brush)
        {
            brush.DrawFormat("<label class=\"col-lg-2 col-md-3 col-form-label m--align-{0} m--padding-0 m--margin-top-15  m--margin-bottom-15\">", this.Align);
            this.Content.Render(brush);
            brush.Draw("</label>");
        }

    }
}
