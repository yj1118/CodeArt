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
    public class SelectSign : Control
    {
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(SelectSign),new PropertyMetadata(false));

        public bool Selected
        {
            get
            {
                return (bool)GetValue(SelectedProperty);
            }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }

        public static readonly DependencyProperty IsRadioProperty = DependencyProperty.Register("IsRadio", typeof(bool), typeof(SelectSign), new PropertyMetadata(false));

        public bool IsRadio
        {
            get
            {
                return (bool)GetValue(IsRadioProperty);
            }
            set
            {
                SetValue(IsRadioProperty, value);
            }
        }

        public SelectSign()
        {
            this.DefaultStyleKey = typeof(SelectSign);
        }

        private Image _image;
        private Border _border;
        private Border _background;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _image = GetTemplateChild("image") as Image;
            _border = GetTemplateChild("border") as Border;
            _background = GetTemplateChild("background") as Border;
        }

    }
}