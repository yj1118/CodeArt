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
    public class DrawerTip : Control
    {
        public DrawerTip()
        {
            this.DefaultStyleKey = typeof(DrawerTip);
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(string), typeof(DrawerTip));

        /// <summary>
        /// 消息的内容
        /// </summary>
        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }


        public static readonly DependencyProperty TextMaxWidthProperty = DependencyProperty.Register("TextMaxWidth", typeof(double), typeof(DrawerTip),new PropertyMetadata((double)500));

        /// <summary>
        /// 显示文本的宽度，利用该值可以达到单行文本或者双行文本的效果
        /// </summary>
        public double TextMaxWidth
        {
            get { return (double)GetValue(TextMaxWidthProperty); }
            set { SetValue(TextMaxWidthProperty, value); }
        }


        public static readonly DependencyProperty ImageSrcProperty = DependencyProperty.Register("ImageSrc", typeof(string), typeof(DrawerTip), new PropertyMetadata("/Playstation/Work/Images/tip.png"));

        public string ImageSrc
        {
            get { return (string)GetValue(ImageSrcProperty); }
            set { SetValue(ImageSrcProperty, value); }
        }

        public static readonly DependencyProperty ImageBackgroundProperty = DependencyProperty.Register("ImageBackground", typeof(Brush), typeof(DrawerTip), new PropertyMetadata(Util.GetBrush("#fff")));

        public Brush ImageBackground
        {
            get { return (Brush)GetValue(ImageBackgroundProperty); }
            set { SetValue(ImageBackgroundProperty, value); }
        }
    }
}