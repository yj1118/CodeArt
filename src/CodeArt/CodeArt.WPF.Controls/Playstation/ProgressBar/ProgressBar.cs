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
    public class ProgressBar : Control
    {
        public ProgressBar()
        {
            this.DefaultStyleKey = typeof(ProgressBar);
        }


        private TextBlock description;
        private TextBlock tip;
        private Border background;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            description = GetTemplateChild("description") as TextBlock;
            tip = GetTemplateChild("tip") as TextBlock;
            background = GetTemplateChild("background") as Border;
        }

        public static readonly DependencyProperty ShowTipProperty = DependencyProperty.Register("ShowTip", typeof(bool), typeof(ProgressBar),new PropertyMetadata(true));

        /// <summary>
        /// 
        /// </summary>
        public bool ShowTip
        {
            get
            {
                return (bool)GetValue(ShowTipProperty);
            }
            set
            {
                SetValue(ShowTipProperty, value);
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ProgressBar));

        /// <summary>
        /// 0-1范围的值
        /// </summary>
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(ProgressBar));

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        public static readonly DependencyProperty ContainerBorderThicknessProperty = DependencyProperty.Register("ContainerBorderThickness", typeof(Thickness), typeof(ProgressBar), new PropertyMetadata(new Thickness(2)));

        /// <summary>
        /// 
        /// </summary>
        public Thickness ContainerBorderThickness
        {
            get
            {
                return (Thickness)GetValue(ContainerBorderThicknessProperty);
            }
            set
            {
                SetValue(ContainerBorderThicknessProperty, value);
            }
        }

        public static readonly DependencyProperty BarSizeProperty = DependencyProperty.Register("BarSize", typeof(double), typeof(ProgressBar), new PropertyMetadata((double)6));

        /// <summary>
        /// 
        /// </summary>
        public double BarSize
        {
            get
            {
                return (double)GetValue(BarSizeProperty);
            }
            set
            {
                SetValue(BarSizeProperty, value);
            }
        }
    }
}