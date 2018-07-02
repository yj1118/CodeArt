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
    public class Slide : FrameworkElement
    {
        public static DependencyProperty DelayProperty;

        public static DependencyProperty ImageProperty;

        static Slide()
        {
            var delayMetadata = new PropertyMetadata(() => { return 5000; });
            DelayProperty = DependencyProperty.Register<int, Slide>("Delay", delayMetadata);

            var imageMetadata = new PropertyMetadata(() => { return "/"; });
            ImageProperty = DependencyProperty.Register<string, Slide>("Image", imageMetadata);
        }

        public int Delay
        {
            get
            {
                return (int)GetValue(DelayProperty);
            }
            set
            {
                SetValue(DelayProperty, value);
            }
        }

        public string Image
        {
            get
            {
                return (string)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }

        protected override void Draw(PageBrush brush)
        {
            brush.Draw(string.Format("<div class=\"ls-slide\" data-ls=\"slidedelay: {0}; transition2d: all;\">", this.Delay));
            brush.Draw(string.Format("<img src=\"{0}\" class=\"ls-bg\">", this.Image));
            brush.DrawLine("</div>");
        }
    }
}
