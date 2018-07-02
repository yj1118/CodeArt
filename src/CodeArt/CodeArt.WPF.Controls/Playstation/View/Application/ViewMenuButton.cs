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
    public class ViewMenuButton : Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ViewMenuButton));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ImageSrcProperty = DependencyProperty.Register("ImageSrc", typeof(string), typeof(ViewMenuButton));

        public string ImageSrc
        {
            get { return (string)GetValue(ImageSrcProperty); }
            set { SetValue(ImageSrcProperty, value); }
        }


        public ViewMenuButton()
        {
            this.MouseUp += OnMouseUp;
            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_isOpened) return;
            this.Status = FloatStatus.LoseFocus;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_isOpened) return;
            this.Status = FloatStatus.Focus;
        }

        private ViewContainer _container;
        private bool _isOpened = false;

        public void Init(ViewContainer container)
        {
            _container = container;
            _container.ViewChanged += OnViewChanged;
        }

        private void OnViewChanged(View old, View current)
        {
            if (old != null && old.ViewTextName == this.Text)
            {
                _isOpened = false;
                this.Status = FloatStatus.LoseFocus;
            }
            else if (current != null && current.ViewTextName == this.Text)
            {
                _isOpened = true;
                this.Status = FloatStatus.Focus;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_isOpened) return;
            _container.OpenView(this.Name);
        }


        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(FloatStatus), typeof(ViewMenuButton), new PropertyMetadata(FloatStatus.LoseFocus));

        /// <summary>
        /// 悬浮状态
        /// </summary>
        public FloatStatus Status
        {
            get { return (FloatStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }
    }
}
