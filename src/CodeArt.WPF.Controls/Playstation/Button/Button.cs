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
    public class Button : Control
    {
        private Border border;
        private TextBlock content;
        private SolidColorBrush borderBrush;
        private Border background;

        public Button()
        {
            this.DefaultStyleKey = typeof(Button);
            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;
            this.MouseDown += OnMouseEnter;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            border = this.GetTemplateChild("border") as Border;
            content = this.GetTemplateChild("content") as TextBlock;
            borderBrush = this.GetTemplateChild("borderBrush") as SolidColorBrush;
            background = this.GetTemplateChild("background") as Border;

            SizeTypeChanged(this, new DependencyPropertyChangedEventArgs(SizeTypeProperty, this.SizeType, this.SizeType));
            StatusChanged(this, new DependencyPropertyChangedEventArgs(StatusProperty, this.Status, this.Status));
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (this.IsDisabled) return;
            this.Status = ButtonStatus.Focus;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (this.IsDisabled) return;
            this.Status = ButtonStatus.Enabled;
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Button));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #region 尺寸的类型

        public static readonly DependencyProperty SizeTypeProperty;

        public ButtonSize SizeType
        {
            get { return (ButtonSize)GetValue(SizeTypeProperty); }
            set { SetValue(SizeTypeProperty, value); }
        }

        private static void SizeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var btn = (Button)d;
            if (btn.content == null) return;
            switch (e.NewValue)
            {
                case ButtonSize.md:
                    {
                        btn.Height = 75;
                        btn.content.FontSize = 25;
                    }
                    break;
            }
        }

        #endregion

        public static readonly DependencyProperty StatusProperty;

        /// <summary>
        /// 按钮的活动状态
        /// </summary>
        public ButtonStatus Status
        {
            get { return (ButtonStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        private static void StatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var btn = (Button)d;
            if (btn.content == null) return;
            var status = (ButtonStatus)e.NewValue;
            switch (status)
            {
                case ButtonStatus.Enabled:
                    {
                        btn.border.BorderThickness = new Thickness(2);
                        btn.borderBrush.Opacity = 0.5;
                        btn.background.Opacity = 0.05;
                        btn.content.Opacity = 0.8;
                    }
                    break;
                case ButtonStatus.Disabled:
                    {
                        btn.border.BorderThickness = new Thickness(2);
                        btn.borderBrush.Opacity = 0.2;
                        btn.background.Opacity = 0;
                        btn.content.Opacity = 0.3;
                    }
                    break;
                case ButtonStatus.Focus:
                    {
                        btn.border.BorderThickness = new Thickness(5);
                        btn.borderBrush.Opacity = 1;
                        btn.background.Opacity = 0;
                        btn.content.Opacity = 1;
                    }
                    break;
            }
        }


        public static readonly DependencyProperty IsDisabledProperty;

        public bool IsDisabled
        {
            get { return (bool)GetValue(IsDisabledProperty); }
            set { SetValue(IsDisabledProperty, value); }
        }

        private static void IsDisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var btn = (Button)d;
            var isDisabled = (bool)e.NewValue;
            if (isDisabled)
            {
                btn.Status = ButtonStatus.Disabled;
            }
            else
            {
                btn.Status = ButtonStatus.Enabled;
            }
        }

        static Button()
        {
            PropertyMetadata sizeTypeMetadata = new PropertyMetadata(SizeTypeChanged);
            SizeTypeProperty = DependencyProperty.Register("SizeType", typeof(ButtonSize), typeof(Button), sizeTypeMetadata);

            PropertyMetadata statusMetadata = new PropertyMetadata(StatusChanged);
            StatusProperty = DependencyProperty.Register("Status", typeof(ButtonStatus), typeof(Button), statusMetadata);

            PropertyMetadata isDisabledMetadata = new PropertyMetadata(IsDisabledChanged);
            IsDisabledProperty = DependencyProperty.Register("IsDisabled", typeof(bool), typeof(Button));
        }

    }
}