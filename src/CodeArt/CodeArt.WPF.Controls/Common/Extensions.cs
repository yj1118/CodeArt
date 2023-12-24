using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Security;
using Microsoft.Win32.SafeHandles;

using CodeArt.WPF.Screen;
using CodeArt.Util;
using CodeArt.WPF;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace CodeArt.WPF.Controls
{
    public static class Extensions
    {
        /// <summary>
        /// 将视觉元素的呈现内容以图片的形式保存
        /// </summary>
        /// <param name="element"></param>
        /// <param name="stream"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SaveImage(this FrameworkElement element, Stream stream,bool undoTransformation = true)
        {
            var dpi = SystemScreen.DPI;
            //得到像素大小
            int width = SystemScreen.GetPhysicalWidth((int)element.Width);
            int height = SystemScreen.GetPhysicalHeight((int)element.Height);

            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, dpi.X, dpi.Y, PixelFormats.Pbgra32);

            if (undoTransformation)
            {
                //以下代码是消除组件被转换过的(比如旋转，比例缩放，或是平移之类)影响
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(element);
                    dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), new System.Windows.Size(element.Width, element.Height)));
                }
                rtb.Render(dv);
            }
            else
            {
                rtb.Render(element);
            }

            using (MemoryStream ms = new MemoryStream())
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                var frame = BitmapFrame.Create(rtb);
                encoder.Frames.Add(frame);
                encoder.Save(ms);

                Compress(ms, stream, width, height);
            }

                
            GC.Collect(); //一定要调用该方法，否则内存泄露，encoder.Save(stream); 方法引起的内存损耗
        }

        private static void Compress(Stream sourceStream, Stream ouputStream, int width, int height)
        {
            System.Drawing.Image source = null;
            Graphics gdiobj = null;
            try
            {
                source = System.Drawing.Image.FromStream(sourceStream);
                using (Bitmap img = new Bitmap(width, height))
                {
                    img.SetResolution(source.HorizontalResolution, source.VerticalResolution);
                    gdiobj = Graphics.FromImage(img);
                    gdiobj.CompositingQuality = CompositingQuality.HighSpeed;
                    gdiobj.SmoothingMode = SmoothingMode.HighSpeed;
                    gdiobj.InterpolationMode = InterpolationMode.Low;
                    gdiobj.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    gdiobj.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(0, System.Drawing.Color.Transparent)), 0, 0, width, height);
                    Rectangle destrect = new Rectangle(0, 0, width, height);
                    gdiobj.DrawImage(source, destrect, 0, 0, source.Width,
                                    source.Height, GraphicsUnit.Pixel);

                    using (System.Drawing.Imaging.EncoderParameters ep = new System.Drawing.Imaging.EncoderParameters(1))
                    {
                        ep.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)1);
                        img.Save(ouputStream, ImageFormat.Png);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (source != null) source.Dispose();
                if (gdiobj != null) gdiobj.Dispose();
            }
        }
    }
}
