using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using CodeArt.Util;
using CodeArt.WPF.UI;

namespace CodeArt.WPF.Controls.Playstation
{
    public partial class PagesMenuButton : ViewMenuButton
    {
        public PagesMenuButton()
        {
            this.DefaultStyleKey = typeof(PagesMenuButton);

            ScaleTransform scale = new ScaleTransform();
            this.RenderTransform = scale;
            this.RenderTransformOrigin = new Point(0.5, 0.5);
        }
    }
}
