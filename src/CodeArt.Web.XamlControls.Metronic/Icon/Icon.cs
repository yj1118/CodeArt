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
    public class Icon : ContentControl
    {
        public static DependencyProperty SrcProperty { get; private set; }

        static Icon()
        {
            var srcMetadata = new PropertyMetadata(() => { return string.Empty; });
            SrcProperty = DependencyProperty.Register<string, Icon>("Src", srcMetadata);

        }

        /// <summary>
        /// 图标来源
        /// </summary>
        public string Src
        {
            get
            {
                return GetValue(SrcProperty) as string;
            }
            set
            {
                SetValue(SrcProperty, value);
            }
        }

        public Icon()
        {
        }

        protected override void Draw(PageBrush brush)
        {
            brush.Draw(string.Format("<i class=\"fa fa-{0}\"></i> ", this.Src.Trim()));
        }
    }
}
