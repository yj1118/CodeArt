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
    public class ButtonMenu : Control
    {
        private TextBlock tip;

        public ButtonMenu()
        {
            this.DefaultStyleKey = typeof(ButtonMenu);

            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Bottom;

            this.Margin = new Thickness(25, 0, 0, 25);
            //this.Opacity = 0.5;
            //this.MouseEnter += OnMouseEnter;
            //this.MouseLeave += OnMouseLeave;
            this.MouseUp += OnMouseUp;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            tip = GetTemplateChild("tip") as TextBlock;

            if (!string.IsNullOrEmpty(this.Text))
            {
                this.tip.Text = this.Text;
                this.tip.Visibility = Visibility.Visible;
            }
        }


        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ButtonMenu));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }




        public static readonly DependencyProperty DrawerProperty = DependencyProperty.Register("Drawer", typeof(object), typeof(ButtonMenu));

        public object Drawer
        {
            get { return GetValue(DrawerProperty); }
            set { SetValue(DrawerProperty, value); }
        }


        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenMenu();
        }

        private void OpenMenu()
        {
            if (this.Drawer == null) return;
            Work.Current.OpenLeft(this.Drawer);
        }

        private void CloseMenu()
        {
            if (this.Drawer == null) return;
            Work.Current.CloseLeft();
        }


        //private void OnMouseEnter(object sender, MouseEventArgs e)
        //{
        //    this.Opacity = 1;
        //}

        //private void OnMouseLeave(object sender, MouseEventArgs e)
        //{
        //    this.Opacity = 0.5;
        //}

    }
}