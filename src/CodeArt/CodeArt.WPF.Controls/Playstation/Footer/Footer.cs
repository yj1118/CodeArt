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
    public class Footer : ContentControl
    {
        private UIElement back;

        public Footer()
        {
            this.DefaultStyleKey = typeof(Footer);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            back = GetTemplateChild("back") as UIElement;
            back.MouseUp += OnBack;
        }

        private void OnBack(object sender, MouseButtonEventArgs e)
        {
            if (BackAction != null)
            {
                BackAction();
                return;
            }

            if (Work.Current != null)
                Work.Current.Back();
        }

        public Action BackAction
        {
            get;
            set;
        }


        public static readonly DependencyProperty ShowBackProperty = DependencyProperty.Register("ShowBack", typeof(bool), typeof(Footer),new PropertyMetadata(true));

        public bool ShowBack
        {
            get
            {
                return (bool)GetValue(ShowBackProperty);
            }
            set
            {
                SetValue(ShowBackProperty, value);
            }
        }

    }
}