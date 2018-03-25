using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// OpacityCard.xaml 的交互逻辑
    /// </summary>
    public partial class OpacityCard : ContentControl
    {
        public OpacityCard()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(OpacityCard));

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OpacityCard));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(RangeStatus), typeof(OpacityCard));

        /// <summary>
        /// 区域的活动状态
        /// </summary>
        public RangeStatus Status
        {
            get { return (RangeStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        
        public static readonly DependencyProperty CardWidthProperty = DependencyProperty.Register("CardWidth", typeof(double), typeof(OpacityCard), new PropertyMetadata(300.0));

        public double CardWidth
        {
            get { return (double)GetValue(CardWidthProperty); }
            set { SetValue(CardWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageContainerHeightProperty = DependencyProperty.Register("ImageContainerHeight", typeof(double), typeof(OpacityCard), new PropertyMetadata(240.0));

        public double ImageContainerHeight
        {
            get { return (double)GetValue(ImageContainerHeightProperty); }
            set { SetValue(ImageContainerHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(OpacityCard), new PropertyMetadata(230.0));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }


        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(OpacityCard), new PropertyMetadata(160.0));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty TextContainerHeightProperty = DependencyProperty.Register("TextContainerHeight", typeof(double), typeof(OpacityCard), new PropertyMetadata(90.0));

        public double TextContainerHeight
        {
            get { return (double)GetValue(TextContainerHeightProperty); }
            set { SetValue(TextContainerHeightProperty, value); }
        }

        public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.Register("TextFontSize", typeof(int), typeof(OpacityCard), new PropertyMetadata(28));

        public int TextFontSize
        {
            get { return (int)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(int), typeof(OpacityCard), new PropertyMetadata(20));

        public int TextMargin
        {
            get { return (int)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

    }

    public class IntTextMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var margin = (int)value;

            return new Thickness(margin, 0, margin, 0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public readonly static IntTextMarginConverter Instance = new IntTextMarginConverter();

    }

}
