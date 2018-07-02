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
    public class Line : Control
    {
        public static readonly DependencyProperty LineOpacityProperty = DependencyProperty.Register("LineOpacity", typeof(double), typeof(Line),new PropertyMetadata(0.3));

        public double LineOpacity
        {
            get { return (double)GetValue(LineOpacityProperty); }
            set { SetValue(LineOpacityProperty, value); }
        }

        public Line()
        {
            this.DefaultStyleKey = typeof(Line);
        }
    }
}