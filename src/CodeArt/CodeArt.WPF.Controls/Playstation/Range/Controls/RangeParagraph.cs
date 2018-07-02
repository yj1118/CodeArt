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
    public class RangeParagraph : Control
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(RangeParagraph));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RangeParagraph));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register("TitleFontSize", typeof(double), typeof(RangeParagraph),new PropertyMetadata(double.NaN));

        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }
        

        public static readonly DependencyProperty SubtextProperty = DependencyProperty.Register("Subtext", typeof(string), typeof(RangeParagraph));

        public string Subtext
        {
            get { return (string)GetValue(SubtextProperty); }
            set { SetValue(SubtextProperty, value); }
        }

        public static readonly DependencyProperty ImageSrcProperty = DependencyProperty.Register("ImageSrc", typeof(string), typeof(RangeParagraph), new PropertyMetadata(string.Empty));

        public string ImageSrc
        {
            get { return (string)GetValue(ImageSrcProperty); }
            set { SetValue(ImageSrcProperty, value); }
        }


        public RangeParagraph()
        {
            this.DefaultStyleKey = typeof(RangeParagraph);
        }

       
    }
}