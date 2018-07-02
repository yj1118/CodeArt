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
    public class DesktopViewHeader : ContentControl
    {
        public DesktopViewHeader()
        {
            this.DefaultStyleKey = typeof(DesktopViewHeader);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(DesktopViewHeader));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty GapProperty = DependencyProperty.Register("Gap", typeof(GridLength), typeof(DesktopViewHeader));

        public GridLength Gap
        {
            get { return (GridLength)GetValue(GapProperty); }
            set { SetValue(GapProperty, value); }
        }

    }
}
