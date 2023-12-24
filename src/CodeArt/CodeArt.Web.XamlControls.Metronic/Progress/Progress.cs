using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.DTO;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Progress.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Progress : Input
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register<string, Progress>("Color", () => { return string.Empty; });

        /// <summary>
        /// success,warning,info,danger等颜色
        /// </summary>
        public string Color
        {
            get
            {
                return (string)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register<string, Progress>("Size", () => { return "sm"; });

        /// <summary>
        /// sm,lg ，系统预设高度，有预设高度会有圆角效果，自定义高度没有
        /// </summary>
        public string Size
        {
            get
            {
                return (string)GetValue(SizeProperty);
            }
            set
            {
                SetValue(SizeProperty, value);
            }
        }

        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register<string, Progress>("Height", () => { return string.Empty; });

        /// <summary>
        /// 自定义高度
        /// </summary>
        public string Height
        {
            get
            {
                return (string)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }

        protected override void Draw(PageBrush brush)
        {
            if(!string.IsNullOrEmpty(this.Height)){
                brush.DrawFormat("<div class=\"progress\" style=\"height: {0};\" ", this.Height);
                brush.Draw(" data-proxy=\"{give:new $$metronic.progress()}\">");
            }
            else
            {
                brush.DrawFormat("<div class=\"progress m-progress--{0}\"", this.Size);
                brush.Draw(" data-proxy=\"{give:new $$metronic.progress()}\">");
            }
            
            brush.DrawLine();
            brush.DrawFormat("<div class=\"progress-bar m--bg-{0}\" role=\"progressbar\" style=\"width: 0%;\" aria-valuenow=\"0\" aria-valuemin=\"0\" aria-valuemax=\"100\"></div>", this.Color);
            brush.DrawLine();
            brush.Draw("</div>");
        }

    }
}
