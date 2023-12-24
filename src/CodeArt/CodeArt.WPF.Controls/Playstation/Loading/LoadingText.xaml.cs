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

namespace CodeArt.WPF.Controls.Playstation
{
    /// <summary>
    /// LoadingText.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingText : UserControl
    {
        public LoadingText()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(LoadingText),new PropertyMetadata((double)1));

        public double IconSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }


        public static readonly DependencyProperty MessageFontSizeProperty = DependencyProperty.Register("MessageFontSize", typeof(double), typeof(LoadingText), new PropertyMetadata((double)28));

        public double MessageFontSize
        {
            get { return (double)GetValue(MessageFontSizeProperty); }
            set { SetValue(MessageFontSizeProperty, value); }
        }

        public static readonly DependencyProperty MessageMarginProperty = DependencyProperty.Register("MessageMargin", typeof(Thickness), typeof(LoadingText), new PropertyMetadata(new Thickness(30, 0, 0, 0)));

        public Thickness MessageMargin
        {
            get { return (Thickness)GetValue(MessageMarginProperty); }
            set { SetValue(MessageMarginProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(LoadingText));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
    }
}