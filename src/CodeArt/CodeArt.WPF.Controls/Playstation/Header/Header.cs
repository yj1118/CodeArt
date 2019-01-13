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
    public class Header : Control
    {
        public Header()
        {
            this.DefaultStyleKey = typeof(Header);
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Header));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        public static readonly DependencyProperty TitleMarginProperty = DependencyProperty.Register("TitleMargin", typeof(Thickness), typeof(Header), new PropertyMetadata(new Thickness(110, 75, 0, 10)));

        public Thickness TitleMargin
        {
            get { return (Thickness)GetValue(TitleMarginProperty); }
            set { SetValue(TitleMarginProperty, value); }
        }

        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register("TitleFontSize", typeof(double), typeof(Header), new PropertyMetadata((double)50));

        public double TitleFontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

    }
}