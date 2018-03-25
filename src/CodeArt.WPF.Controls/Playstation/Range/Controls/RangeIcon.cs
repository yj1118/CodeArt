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
    public class RangeIcon : Control
    {
        public RangeIcon()
        {
            this.DefaultStyleKey = typeof(RangeIcon);
        }

        public static DependencyProperty FocusBorderThicknessProperty = DependencyProperty.Register("FocusBorderThickness", typeof(Thickness), typeof(RangeIcon), new PropertyMetadata(default(Thickness)));

        public Thickness FocusBorderThickness
        {
            get
            {
                return (Thickness)GetValue(FocusBorderThicknessProperty);
            }
            set
            {
                SetValue(FocusBorderThicknessProperty, value);
            }
        }


        public static DependencyProperty RawBorderOpacityProperty = DependencyProperty.Register("RawBorderOpacity", typeof(double), typeof(RangeIcon), new PropertyMetadata((double)0.5));

        public double RawBorderOpacity
        {
            get
            {
                return (double)GetValue(RawBorderOpacityProperty);
            }
            set
            {
                SetValue(RawBorderOpacityProperty, value);
            }
        }


        public static DependencyProperty RawBorderThicknessProperty = DependencyProperty.Register("RawBorderThickness", typeof(Thickness), typeof(RangeIcon), new PropertyMetadata(new Thickness(2)));

        public Thickness RawBorderThickness
        {
            get
            {
                return (Thickness)GetValue(RawBorderThicknessProperty);
            }
            set
            {
                SetValue(RawBorderThicknessProperty, value);
            }
        }

        public static DependencyProperty RawInnerBorderThicknessProperty = DependencyProperty.Register("RawInnerBorderThickness", typeof(Thickness), typeof(RangeIcon), new PropertyMetadata(new Thickness(0)));

        public Thickness RawInnerBorderThickness
        {
            get
            {
                return (Thickness)GetValue(RawInnerBorderThicknessProperty);
            }
            set
            {
                SetValue(RawInnerBorderThicknessProperty, value);
            }
        }


        public static readonly DependencyProperty ImageSrcProperty = DependencyProperty.Register("ImageSrc", typeof(string), typeof(RangeIcon),new PropertyMetadata(string.Empty));

        public string ImageSrc
        {
            get { return (string)GetValue(ImageSrcProperty); }
            set { SetValue(ImageSrcProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(RangeIcon));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(RangeIcon));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }


        public static readonly DependencyProperty ShowModeProperty = ShowModeProperty = DependencyProperty.Register("ShowMode", typeof(RangeMode), typeof(RangeIcon), new PropertyMetadata(RangeMode.Border));


        /// <summary>
        /// 区域的显示模式
        /// </summary>
        public RangeMode ShowMode
        {
            get { return (RangeMode)GetValue(ShowModeProperty); }
            set { SetValue(ShowModeProperty, value); }
        }

        public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register("BackgroundOpacity", typeof(double), typeof(RangeIcon),new PropertyMetadata(0.05));

        public double BackgroundOpacity
        {
            get { return (double)GetValue(BackgroundOpacityProperty); }
            set { SetValue(BackgroundOpacityProperty, value); }
        }


        static RangeIcon()
        {

        }

        private Range range;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            range = GetTemplateChild("range") as Range;
        }

        public static readonly DependencyProperty IsFiexdFocusProperty = DependencyProperty.Register("IsFiexdFocus", typeof(bool), typeof(RangeIcon), new PropertyMetadata(false, IsFiexdFocusChanged));


        public bool IsFiexdFocus
        {
            get { return (bool)GetValue(IsFiexdFocusProperty); }
            set { SetValue(IsFiexdFocusProperty, value); }
        }

        private static void IsFiexdFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (RangeIcon)d;
            var value = (bool)e.NewValue;
            if (obj.range == null) obj.range.ApplyTemplate();
            obj.range.Status = value ? RangeStatus.FixedFocus : RangeStatus.Enabled;
        }
    }
}