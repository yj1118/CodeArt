using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace CodeArt.WPF.Screen
{
    public struct ScreenDPI
    {
        public double X;
        public double Y;

        public double ScalingX
        {
            get
            {
                return this.X / 96;
            }
        }
        public double ScalingY
        {
            get
            {
                return this.Y / 96;
            }
        }

        //public double Scaling
        //{
        //    get
        //    {
        //        return this.ScalingX; //此处做的预留，理论来讲缩放x和缩放y应该是一样的，但是防止特别情况，做了预留
        //    }
        //}
    }
}
