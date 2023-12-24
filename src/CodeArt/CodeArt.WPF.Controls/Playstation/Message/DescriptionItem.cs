using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CodeArt.Util;

namespace CodeArt.WPF.Controls.Playstation
{
    public class DescriptionItem : ObservableObject
    {
        public string Text
        {
            get;
            set;
        }

        public string ImageSrc
        {
            get;
            set;
        }

        public double ImageWidth
        {
            get;
            set;
        }

        public double ImageHeight
        {
            get;
            set;
        }
        public Thickness TextMargin
        {
            get;
            set;
        }
        public double FontSize
        {
            get;
            set;
        }
        public FontWeight FontWeight
        {
            get;
            set;
        }

        public DescriptionItem()
        {
            this.Text = string.Empty;
            this.ImageSrc = string.Empty;
            this.ImageWidth = 90;
            this.ImageHeight = 90;
            this.TextMargin = new Thickness(40, 80, 40, 80);
            this.FontSize = 28;
            this.FontWeight = FontWeights.Light;
        }
    }
}
