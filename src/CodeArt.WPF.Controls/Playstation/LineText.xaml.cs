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
    /// LineText.xaml 的交互逻辑
    /// </summary>
    public partial class LineText : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(LineText));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty LineOpacityProperty = DependencyProperty.Register("LineOpacity", typeof(double), typeof(LineText), new PropertyMetadata(0.3));

        public double LineOpacity
        {
            get { return (double)GetValue(LineOpacityProperty); }
            set { SetValue(LineOpacityProperty, value); }
        }

        public static readonly DependencyProperty TextOpacityProperty = DependencyProperty.Register("TextOpacity", typeof(double), typeof(LineText), new PropertyMetadata(0.5));

        public double TextOpacity
        {
            get { return (double)GetValue(TextOpacityProperty); }
            set { SetValue(TextOpacityProperty, value); }
        }


        public LineText()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
