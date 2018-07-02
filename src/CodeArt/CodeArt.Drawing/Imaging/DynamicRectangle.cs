using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CodeArt.Drawing.Imaging
{
    /// <summary>
    /// 表示可以计算缩放、裁剪的矩形区域
    /// </summary>
    public struct DynamicRectangle
    {
        public int Width;
        public int Height;
        public int X;
        public int Y;

        /// <summary>
        /// 将原始长宽缩放到指定的大小
        /// 这时得到的是图像在指定区域中实际的偏移量和大小
        /// </summary>
        /// <param name="targetWidth"></param>
        /// <param name="targetHeight"></param>
        /// <param name="isEnlarge">当源图片小于需要展现的区域大小时，是否需要将源图片放大</param>
        /// <returns></returns>
        public DynamicRectangle Scale(int targetWidth, int targetHeight,bool isEnlarge = false)
        {
            if (targetWidth < 0 || targetHeight < 0) throw new DrawingException("DynamicRectangle 参数异常");
            if (targetWidth == 0 && targetHeight == 0) return this;

            DynamicRectangle result = new DynamicRectangle();
            result.X = 0;
            result.Y = 0;

            //先等比缩放
            if (this.Width > this.Height)
            {
                //宽大于高，以宽等比缩放
                if (isEnlarge)
                {
                    result.Width = targetWidth;
                    result.Height = (int)(this.Height * (double)result.Width / (double)this.Width);

                    if (result.Height > targetHeight)//得到的结果比需要的高度高，则根据高度缩放
                    {
                        result.Height = targetHeight;
                        result.Width = (int)(this.Width * (double)result.Height / (double)this.Height);
                    }
                }
                else
                {
                    result.Width = this.Width > targetWidth ? targetWidth : this.Width;
                    result.Height = (int)(this.Height * (double)result.Width / (double)this.Width);

                    if (result.Height > targetHeight)//得到的结果比需要的高度高，则根据高度缩放
                    {
                        result.Height = this.Height > targetHeight ? targetHeight : this.Height;
                        result.Width = (int)(this.Width * (double)result.Height / (double)this.Height);
                    }
                }
                
            }
            else
            {
                //高大于宽，以高等比缩放
                if (isEnlarge)
                {
                    result.Height = targetHeight;
                    result.Width = (int)(this.Width * (double)result.Height / (double)this.Height);
                    if (result.Width > targetWidth)//得到的结果比需要的宽度宽，则根据宽度缩放
                    {
                        result.Width = targetWidth;
                        result.Height = (int)(this.Height * (double)result.Width / (double)this.Width);
                    }
                }
                else
                {
                    result.Height = this.Height > targetHeight ? targetHeight : this.Height;
                    result.Width = (int)(this.Width * (double)result.Height / (double)this.Height);

                    if (result.Width > targetWidth)//得到的结果比需要的宽度宽，则根据宽度缩放
                    {
                        result.Width = this.Width > targetWidth ? targetWidth : this.Width;
                        result.Height = (int)(this.Height * (double)result.Width / (double)this.Width);
                    }
                }
            }

            if (result.Width < targetWidth)
                result.X = (targetWidth - result.Width) / 2;
            if (result.Height < targetHeight)
                result.Y = (targetHeight - result.Height) / 2;

            return result;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(this.X, this.Y, this.Width, this.Height);
        }

    }
}
