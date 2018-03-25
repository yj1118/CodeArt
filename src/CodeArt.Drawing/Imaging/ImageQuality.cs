using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace CodeArt.Drawing.Imaging
{
    /// <summary>
    /// 图片质量
    /// </summary>
    public struct ImageQuality
    {
        /// <summary>
        /// 指定在复合期间使用的质量等级
        /// </summary>
        public CompositingQuality Compositing;

        /// <summary>
        /// 指定是否将平滑处理（抗锯齿）应用于直线、曲线和已填充区域的边缘。
        /// </summary>
        public SmoothingMode Smoothing;

        /// <summary>
        /// 指定在缩放或旋转图像时使用的算法
        /// </summary>
        public InterpolationMode Interpolation;

        /// <summary>
        /// 指定在呈现期间像素偏移的方式
        /// </summary>
        public PixelOffsetMode PixelOffset;

        /// <summary>
        /// 最高画质
        /// </summary>
        public static readonly ImageQuality Height = new ImageQuality()
        {
            Compositing = CompositingQuality.HighQuality,
            Smoothing = SmoothingMode.HighQuality,
            Interpolation = InterpolationMode.HighQualityBicubic,
            PixelOffset = PixelOffsetMode.HighQuality
        };

        /// <summary>
        /// 高速度、低质量
        /// </summary>
        public static readonly ImageQuality Low = new ImageQuality()
        {
            Compositing = CompositingQuality.HighSpeed,
            Smoothing = SmoothingMode.HighSpeed,
            Interpolation = InterpolationMode.Low,
            PixelOffset = PixelOffsetMode.HighSpeed
        };

    }
}
