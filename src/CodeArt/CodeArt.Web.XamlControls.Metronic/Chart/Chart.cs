using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.DTO;
using CodeArt.ModuleNest;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Chart.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Chart : ContentControl
    {
        /// <summary>
        /// 图表高度，只用指定高度，宽度由系统自动计算
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register<int, Chart>("Height", () => { return 600; });


        public int Height
        {
            get
            {
                return (int)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }



        public Chart()
        {

        }
    }
}
