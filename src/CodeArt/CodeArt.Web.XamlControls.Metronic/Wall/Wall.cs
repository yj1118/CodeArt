using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.XamlControls.Metronic
{
    [TemplateCode("Template", "CodeArt.Web.XamlControls.Metronic.Wall.Template.html,CodeArt.Web.XamlControls.Metronic")]
    public class Wall : ContentControl
    {
        public static DependencyProperty TitleProperty = DependencyProperty.Register<string, Wall>("Title", new PropertyMetadata(() => { return string.Empty; }));

        public static DependencyProperty ColorProperty = DependencyProperty.Register<string, Wall>("Color", new PropertyMetadata(() => { return string.Empty; }));

        protected override void OnGotClass(ref object baseValue)
        {
            baseValue = string.Format("m-portlet--{0}", this.Color);
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return GetValue(TitleProperty) as string;
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// red,green,yellow
        /// </summary>
        public string Color
        {
            get
            {
                return GetValue(ColorProperty) as string;
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register<string, Wall>("BackgroundColor", () => { return "#f2f3f8"; });

        public string BackgroundColor
        {
            get
            {
                return (string)GetValue(BackgroundColorProperty);
            }
            set
            {
                SetValue(BackgroundColorProperty, value);
            }
        }
    }
}
