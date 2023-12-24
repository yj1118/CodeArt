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
    /// ActionBarButton.xaml 的交互逻辑
    /// </summary>
    public partial class ActionBarButton : ContentControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ActionBarButton));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public ActionBarButton()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.MouseEnter += ActionBarButton_MouseEnter;
            this.MouseLeave += ActionBarButton_MouseLeave;
        }

        private void ActionBarButton_MouseLeave(object sender, MouseEventArgs e)
        {
            this.main.Opacity = 0.7;
        }

        private void ActionBarButton_MouseEnter(object sender, MouseEventArgs e)
        {
            this.main.Opacity = 1;
        }
    }
}
