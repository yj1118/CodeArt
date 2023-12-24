using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using CodeArt.WPF.UI;

namespace CodeArt.WPF.Controls.Playstation
{
    public class ZoomButton : Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ZoomButton));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ImageSrcProperty = DependencyProperty.Register("ImageSrc", typeof(string), typeof(ZoomButton));

        public string ImageSrc
        {
            get { return (string)GetValue(ImageSrcProperty); }
            set { SetValue(ImageSrcProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(FloatStatus), typeof(ZoomButton), new PropertyMetadata(FloatStatus.LoseFocus));

        /// <summary>
        /// 悬浮状态
        /// </summary>
        public FloatStatus Status
        {
            get { return (FloatStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public ZoomButton()
        {
            this.DefaultStyleKey = typeof(ZoomButton);


            ScaleTransform scale = new ScaleTransform();
            this.RenderTransform = scale;
            this.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            this.Status = FloatStatus.LoseFocus;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            this.Status = FloatStatus.Focus;
        }

        static ZoomButton()
        {

        }
    }
}