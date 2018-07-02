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
    public class Mask : Control
    {
        public Mask()
        {
            this.DefaultStyleKey = typeof(Mask);
            this.MouseUp += OnMouseUp;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_clicked != null)
                _clicked();
        }

        private Action _clicked = null;

        public void Open(int zIndex = 10, Action clicked = null)
        {
            this.Opacity = 0;
            this.Visibility = Visibility.Visible;
            Panel.SetZIndex(this, zIndex);

            var config = new Animations.LinearConfig<double>()
            {
                Start = 0,
                End = 0.2,
                DurationMilliseconds = 300,
                EasingMode = EasingMode.EaseOut,
                CallBack = () =>
                {
                    _clicked = clicked;
                }
            };
            Animations.Linear(this, Mask.OpacityProperty, config);
        }

        public void Close()
        {
            var config = new Animations.LinearConfig<double>()
            {
                Start = 0.2,
                End = 0,
                DurationMilliseconds = 300,
                EasingMode = EasingMode.EaseOut,
                CallBack = () =>
                {
                    this.Visibility = Visibility.Collapsed;
                    _clicked = null;
                }
            };
            Animations.Linear(this, Mask.OpacityProperty, config);

        }
    }
}