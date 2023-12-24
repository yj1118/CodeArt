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
    public class DrawerRight : ContentControl
    {
        public DrawerRight()
        {
            this.DefaultStyleKey = typeof(DrawerRight);
        }

        private Grid container;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            container = GetTemplateChild("container") as Grid;
        }

        public void SetContent(UIElement content)
        {
            if(this.container.Children.Count > 0)
            {
                this.container.Children.Clear();
            }
            this.container.Children.Add(content);
        }


    }
}