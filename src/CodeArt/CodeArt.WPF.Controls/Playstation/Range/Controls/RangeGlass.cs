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
    public class RangeGlass : ContentControl
    {
        public static readonly DependencyProperty ShowOpacityProperty = DependencyProperty.Register("ShowOpacity", typeof(bool), typeof(RangeGlass), new PropertyMetadata(true));

        public bool ShowOpacity
        {
            get { return (bool)GetValue(ShowOpacityProperty); }
            set { SetValue(ShowOpacityProperty, value); }
        }

        public static DependencyProperty InnerBorderThicknessProperty = DependencyProperty.Register("InnerBorderThickness", typeof(Thickness), typeof(RangeGlass), new PropertyMetadata(new Thickness(2)));

        public Thickness InnerBorderThickness
        {
            get
            {
                return (Thickness)GetValue(InnerBorderThicknessProperty);
            }
            set
            {
                SetValue(InnerBorderThicknessProperty, value);
            }
        }


        public static readonly DependencyProperty ImageSrcProperty = DependencyProperty.Register("ImageSrc", typeof(string), typeof(RangeGlass), new PropertyMetadata(string.Empty));

        public string ImageSrc
        {
            get { return (string)GetValue(ImageSrcProperty); }
            set { SetValue(ImageSrcProperty, value); }
        }



        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(RangeGlass), new PropertyMetadata((double)90));

        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(RangeGlass), new PropertyMetadata((double)90));

        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageContainerHeightProperty = DependencyProperty.Register("ImageContainerHeight", typeof(double), typeof(RangeGlass), new PropertyMetadata((double)130));

        public double ImageContainerHeight
        {
            get { return (double)GetValue(ImageContainerHeightProperty); }
            set { SetValue(ImageContainerHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageContainerWidthProperty = DependencyProperty.Register("ImageContainerWidth", typeof(double), typeof(RangeGlass), new PropertyMetadata((double)130));

        public double ImageContainerWidth
        {
            get { return (double)GetValue(ImageContainerWidthProperty); }
            set { SetValue(ImageContainerWidthProperty, value); }
        }


        public RangeGlass()
        {
            this.DefaultStyleKey = typeof(RangeGlass);
        }

       
    }
}