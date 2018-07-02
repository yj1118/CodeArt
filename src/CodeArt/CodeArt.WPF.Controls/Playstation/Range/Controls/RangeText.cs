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
    public class RangeText : Control
    {
        public RangeText()
        {
            this.DefaultStyleKey = typeof(RangeText);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(RangeText));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        public static readonly DependencyProperty ImageSrcProperty = DependencyProperty.Register("ImageSrc", typeof(string), typeof(RangeText),new PropertyMetadata(string.Empty));

        public string ImageSrc
        {
            get { return (string)GetValue(ImageSrcProperty); }
            set { SetValue(ImageSrcProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(RangeText));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(RangeText));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register("TextMargin", typeof(Thickness), typeof(RangeText), new PropertyMetadata(new Thickness(15, 0, 0, 0)));

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        #region 副文本


        public static readonly DependencyProperty SubtextProperty = DependencyProperty.Register("Subtext", typeof(string), typeof(RangeText));

        public string Subtext
        {
            get { return (string)GetValue(SubtextProperty); }
            set { SetValue(SubtextProperty, value); }
        }


        public static readonly DependencyProperty SubtextMarginProperty = DependencyProperty.Register("SubtextMargin", typeof(Thickness), typeof(RangeText), new PropertyMetadata(new Thickness(15, 0, 0, 0)));

        public Thickness SubtextMargin
        {
            get { return (Thickness)GetValue(SubtextMarginProperty); }
            set { SetValue(SubtextMarginProperty, value); }
        }

        public static readonly DependencyProperty SubFontSizeProperty = DependencyProperty.Register("SubFontSize", typeof(double), typeof(RangeText));

        public double SubFontSize
        {
            get { return (double)GetValue(SubFontSizeProperty); }
            set { SetValue(SubFontSizeProperty, value); }
        }

        public static readonly DependencyProperty SubOpacityProperty = DependencyProperty.Register("SubOpacity", typeof(double), typeof(RangeText),new PropertyMetadata(0.5));

        public double SubOpacity
        {
            get { return (double)GetValue(SubOpacityProperty); }
            set { SetValue(SubOpacityProperty, value); }
        }

        #endregion



        public static readonly DependencyProperty ShowModeProperty = DependencyProperty.Register("ShowMode", typeof(RangeMode), typeof(RangeText), new PropertyMetadata(RangeMode.Border));

        /// <summary>
        /// 区域的显示模式
        /// </summary>
        public RangeMode ShowMode
        {
            get { return (RangeMode)GetValue(ShowModeProperty); }
            set { SetValue(ShowModeProperty, value); }
        }


        public static readonly DependencyProperty ImageVisibilityProperty = DependencyProperty.Register("ImageVisibility", typeof(Visibility), typeof(RangeText), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 
        /// </summary>
        public Visibility ImageVisibility
        {
            get { return (Visibility)GetValue(ImageVisibilityProperty); }
            set { SetValue(ImageVisibilityProperty, value); }
        }

        private Range _range;
        private ImagePro _image;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _image = GetTemplateChild("image") as ImagePro;
            _range = GetTemplateChild("range") as Range;

            this.MakeChildsLoaded(_range);

            IsFiexdFocusChanged(this, new DependencyPropertyChangedEventArgs(IsFiexdFocusProperty, null, false));
        }

        public static readonly DependencyProperty IsFiexdFocusProperty = DependencyProperty.Register("IsFiexdFocus", typeof(bool), typeof(RangeText), new PropertyMetadata(false, IsFiexdFocusChanged));


        public bool IsFiexdFocus
        {
            get { return (bool)GetValue(IsFiexdFocusProperty); }
            set { SetValue(IsFiexdFocusProperty, value); }
        }

        private static void IsFiexdFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var text = (RangeText)d;
            var value = (bool)e.NewValue;
            if (text._range == null) text.ApplyTemplate();
            text._range.Status = value ? RangeStatus.FixedFocus : RangeStatus.Enabled;
        }

        public static readonly DependencyProperty DisabledProperty = DependencyProperty.Register("Disabled", typeof(bool), typeof(RangeText), new PropertyMetadata(false, DisabledChanged));


        public bool Disabled
        {
            get { return (bool)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }

        private static void DisabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var text = (RangeText)d;
            var value = (bool)e.NewValue;
            if (text._range == null) text.ApplyTemplate();
            text._range.Status = value ? RangeStatus.Disabled : RangeStatus.Enabled;
        }

    }
}