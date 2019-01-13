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
    public class Line : Control
    {
        

        public Line()
        {

        }

        protected override void Draw(PageBrush brush)
        {
            brush.DrawLine("<div class=\"m-separator m-separator--dashed m-separator--lg\"></div>");
        }
    }
}
