using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;

namespace CodeArt.Web.XamlControls
{
    public class ScreenCarouselCode : ScriptCode
    {
        public static DependencyProperty SkinsPathProperty { get; private set; }

        static ScreenCarouselCode()
        {
            var skinsPathMetadata = new PropertyMetadata(() => { return string.Empty; });
            SkinsPathProperty = DependencyProperty.Register<string, ScreenCarouselCode>("SkinsPath", skinsPathMetadata);
        }

        public string SkinsPath
        {
            get
            {
                return GetValue(SkinsPathProperty) as string;
            }
            set
            {
                SetValue(SkinsPathProperty, value);
            }
        }


        protected override void FillCode(StringBuilder code)
        {
            if (string.IsNullOrEmpty(this.Id)) throw new XamlException("没有指定Id,不能使用ScreenCarousel控件");

            code.AppendFormat("$(\"#{0}\").layerSlider(",this.Id);
            code.Append("{");
            code.AppendLine();
            code.AppendLine("    pauseOnHover: false,");
            code.AppendFormat("    skinsPath: '{0}',", this.SkinsPath);
            code.AppendLine();
            code.AppendLine("    skin: 'noskin',");
            code.AppendLine("    showCircleTimer: false");
            code.Append("});");
        }
    }
}