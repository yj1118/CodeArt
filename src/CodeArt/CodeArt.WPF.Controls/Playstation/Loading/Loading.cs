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
    [TemplatePart(Name = "Border", Type = typeof(Border))]
    public class Loading : Control
    {
        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(Loading), new PropertyMetadata(1d, (o, e) => {
                Loading li = (Loading)o;

                if (li.PART_Border == null || li.IsActive == false)
                {
                    return;
                }

                foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(li.PART_Border))
                {
                    if (group.Name == "ActiveStates")
                    {
                        foreach (VisualState state in group.States)
                        {
                            if (state.Name == "Active")
                            {
                                state.Storyboard.SetSpeedRatio(li.PART_Border, (double)e.NewValue);
                            }
                        }
                    }
                }
            }));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(Loading), new PropertyMetadata(true, (o, e) => {
                Loading li = (Loading)o;

                if (li.PART_Border == null)
                {
                    return;
                }

                if ((bool)e.NewValue == false)
                {
                    VisualStateManager.GoToElementState(li.PART_Border, "Inactive", false);
                    li.PART_Border.Visibility = Visibility.Collapsed;
                }
                else
                {
                    VisualStateManager.GoToElementState(li.PART_Border, "Active", false);
                    li.PART_Border.Visibility = Visibility.Visible;

                    foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(li.PART_Border))
                    {
                        if (group.Name == "ActiveStates")
                        {
                            foreach (VisualState state in group.States)
                            {
                                if (state.Name == "Active")
                                {
                                    state.Storyboard.SetSpeedRatio(li.PART_Border, li.SpeedRatio);
                                }
                            }
                        }
                    }
                }
            }));


        protected Border PART_Border;

        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }

        /// <summary>
        /// 请使用该属性控制loading的显示与隐藏，这个属性会停止动画，不耗CPU
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Border = (Border)GetTemplateChild("PART_Border");

            if (PART_Border != null)
            {
                VisualStateManager.GoToElementState(PART_Border, (this.IsActive ? "Active" : "Inactive"), false);
                foreach (VisualStateGroup group in VisualStateManager.GetVisualStateGroups(PART_Border))
                {
                    if (group.Name == "ActiveStates")
                    {
                        foreach (VisualState state in group.States)
                        {
                            if (state.Name == "Active")
                            {
                                state.Storyboard.SetSpeedRatio(PART_Border, this.SpeedRatio);
                            }
                        }
                    }
                }

                PART_Border.Visibility = (this.IsActive ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public Loading()
        {
            this.DefaultStyleKey = typeof(Loading);
        }


        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(double), typeof(Loading),new PropertyMetadata((double)1));

        /// <summary>
        /// 大小，以80x80为基准的大小倍数，size=1，则是80x80,size=0.5,这是40x40
        /// </summary>
        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }
    }
}
