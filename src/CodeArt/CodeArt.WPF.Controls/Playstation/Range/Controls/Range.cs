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
    public class Range : ContentControl
    {
        public static DependencyProperty RawBorderThicknessProperty = DependencyProperty.Register("RawBorderThickness", typeof(Thickness), typeof(Range), new PropertyMetadata(new Thickness(5)));

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

        public static DependencyProperty RawBorderOpacityProperty = DependencyProperty.Register("RawBorderOpacity", typeof(double), typeof(Range), new PropertyMetadata((double)0));

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

        public static DependencyProperty FocusBorderOpacityProperty = DependencyProperty.Register("FocusBorderOpacity", typeof(double), typeof(Range), new PropertyMetadata((double)1));

        public double FocusBorderOpacity
        {
            get
            {
                return (double)GetValue(FocusBorderOpacityProperty);
            }
            set
            {
                SetValue(FocusBorderOpacityProperty, value);
            }
        }


        public static DependencyProperty FocusBorderThicknessProperty = DependencyProperty.Register("FocusBorderThickness", typeof(Thickness), typeof(Range), new PropertyMetadata(default(Thickness)));

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



        public static DependencyProperty RawInnerBorderThicknessProperty = DependencyProperty.Register("RawInnerBorderThickness", typeof(Thickness), typeof(Range), new PropertyMetadata(new Thickness(0)));

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

        public static DependencyProperty RawInnerBorderOpacityProperty = DependencyProperty.Register("RawInnerBorderOpacity", typeof(double), typeof(Range), new PropertyMetadata((double)0));

        public double RawInnerBorderOpacity
        {
            get
            {
                return (double)GetValue(RawInnerBorderOpacityProperty);
            }
            set
            {
                SetValue(RawInnerBorderOpacityProperty, value);
            }
        }

        /// <summary>
        /// 存在内边框
        /// </summary>
        public bool ExistInnerBorder
        {
            get
            {
                return this.RawInnerBorderThickness.Top > 0;
            }
        }



        public Range()
        {
            this.DefaultStyleKey = typeof(Range);
            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;
        }

        private Brush borderBrush;
        private Border border;

        private Brush innerBorderBrush;
        private Border innerBorder;


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            border = this.GetTemplateChild("Border") as Border;
            borderBrush = this.GetTemplateChild("BorderBrush") as Brush;
            borderBrush.Opacity = this.RawBorderOpacity;

            innerBorder = this.GetTemplateChild("InnerBorder") as Border;
            innerBorderBrush = this.GetTemplateChild("InnerBorderBrush") as Brush;
            innerBorderBrush.Opacity = this.RawInnerBorderOpacity;

            this.MakeChildsLoaded(border, innerBorder);

            ShowModeChanged(this, new DependencyPropertyChangedEventArgs(ShowModeProperty, null, ShowModeProperty.DefaultMetadata.DefaultValue));
            StatusChanged(this, new DependencyPropertyChangedEventArgs(StatusProperty, null, StatusProperty.DefaultMetadata.DefaultValue));
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Status == RangeStatus.FixedFocus || this.Status == RangeStatus.Disabled) return;
            this.Status = RangeStatus.Focus;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Status == RangeStatus.FixedFocus || this.Status == RangeStatus.Disabled) return;
            this.Status = RangeStatus.Enabled;
        }


        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register("Status", typeof(RangeStatus), typeof(Range), new PropertyMetadata(StatusChanged));

        /// <summary>
        /// 区域的活动状态
        /// </summary>
        public RangeStatus Status
        {
            get { return (RangeStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        private static void StatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var range = (Range)d;
            switch(range.ShowMode)
            {
                case RangeMode.Border:
                    {
                        StatusChangedByBorder(range, e);
                    }
                    break;
                case RangeMode.Opacity:
                    {
                        StatusChangedByOpacity(range, e);
                    }
                    break;
                case RangeMode.Both:
                    {
                        StatusChangedByBorder(range, e);
                        StatusChangedByOpacity(range, e);
                    }
                    break;
            }
        }

        private static void StatusChangedByBorder(Range range, DependencyPropertyChangedEventArgs e)
        {
            if (range.borderBrush == null) range.ApplyTemplate();
            var status = (RangeStatus)e.NewValue;
            if (e.OldValue != null)
            {
                var oldStatus = (RangeStatus)e.OldValue;
                if (IsRepeatAnmimation(status, oldStatus)) return;
            }

            switch (status)
            {
                case RangeStatus.Enabled:
                    {
                        Animations.Opacity(range.borderBrush, SolidColorBrush.OpacityProperty, range.borderBrush.Opacity, range.RawBorderOpacity, 500, EasingMode.EaseOut);
                        if(range.ExistInnerBorder)
                        {
                            Animations.Opacity(range.innerBorderBrush, SolidColorBrush.OpacityProperty, range.innerBorderBrush.Opacity, range.RawInnerBorderOpacity, 500, EasingMode.EaseOut);
                        }

                        if (range.FocusBorderThickness != default(Thickness))
                        {
                            Animations.Linear(range.border, Border.BorderThicknessProperty,
                                new Animations.LinearConfig<Thickness>()
                                {
                                    DurationMilliseconds = 500,
                                    EasingMode = EasingMode.EaseOut,
                                    Start = range.border.BorderThickness,
                                    End = range.RawBorderThickness
                                });
                        }

                    }
                    break;
                case RangeStatus.FixedFocus:
                case RangeStatus.Focus:
                    {
                        Animations.Opacity(range.borderBrush, SolidColorBrush.OpacityProperty, range.borderBrush.Opacity, range.FocusBorderOpacity, 500, EasingMode.EaseOut);
                        if (range.ExistInnerBorder)
                        {
                            //获得焦点时，内边框不显示
                            Animations.Opacity(range.innerBorderBrush, SolidColorBrush.OpacityProperty, range.innerBorderBrush.Opacity, 0, 500, EasingMode.EaseOut);
                        }

                        if (range.FocusBorderThickness != default(Thickness))
                        {
                            Animations.Linear(range.border, Border.BorderThicknessProperty,
                                new Animations.LinearConfig<Thickness>()
                                {
                                    DurationMilliseconds = 500,
                                    EasingMode = EasingMode.EaseOut,
                                    Start = range.border.BorderThickness,
                                    End = range.FocusBorderThickness
                                });
                        }

                    }
                    break;
            }
        }

        private static void StatusChangedByOpacity(Range range, DependencyPropertyChangedEventArgs e)
        {
            var status = (RangeStatus)e.NewValue;
            if (e.OldValue != null)
            {
                var oldStatus = (RangeStatus)e.OldValue;
                if (IsRepeatAnmimation(status, oldStatus)) return;
            }

            switch (status)
            {
                case RangeStatus.Enabled:
                    {
                        Animations.Opacity(range, 1, 0.5, 500, EasingMode.EaseOut);
                    }
                    break;
                case RangeStatus.FixedFocus:
                case RangeStatus.Focus:
                    {
                        Animations.Opacity(range, 0.5, 1, 500, EasingMode.EaseOut);
                    }
                    break;
            }
        }

        /// <summary>
        /// 该方法主要防止切换状态造成的闪烁
        /// </summary>
        /// <param name="status0"></param>
        /// <param name="status1"></param>
        /// <returns></returns>
        private static bool IsRepeatAnmimation(RangeStatus status0, RangeStatus status1)
        {
            if (status0 == status1) return true;
            if (status0 == RangeStatus.Focus && status1 == RangeStatus.FixedFocus) return true;
            if (status0 == RangeStatus.FixedFocus && status1 == RangeStatus.Focus) return true;
            return false;
        }

        public static readonly DependencyProperty ShowModeProperty = ShowModeProperty = DependencyProperty.Register("ShowMode", typeof(RangeMode), typeof(Range), new PropertyMetadata(RangeMode.Border, ShowModeChanged));
    
        /// <summary>
        /// 区域的显示模式
        /// </summary>
        public RangeMode ShowMode
        {
            get { return (RangeMode)GetValue(ShowModeProperty); }
            set { SetValue(ShowModeProperty, value); }
        }

        private static void ShowModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as Range;
            if (o.borderBrush == null) o.ApplyTemplate();
            var @new = (RangeMode)e.NewValue;

            if (@new == RangeMode.Opacity
                || @new == RangeMode.Both)
            {
                if(o.Status == RangeStatus.FixedFocus || o.Status == RangeStatus.Focus)
                {
                    o.Opacity = 1;
                }
                else
                {
                    o.Opacity = 0.5;
                }
            }

            if (@new == RangeMode.Border
                || @new == RangeMode.Both)
            {
                if (o.Status == RangeStatus.FixedFocus || o.Status == RangeStatus.Focus)
                {
                    o.borderBrush.Opacity = 1;
                }
                else
                {
                    o.borderBrush.Opacity = 0;
                }
            }
            else
            {
                o.borderBrush.Opacity = 0;
            }

        }

        static Range()
        {
        }
            
    }
}