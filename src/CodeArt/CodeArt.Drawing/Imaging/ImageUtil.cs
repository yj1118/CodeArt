using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using System.Runtime.InteropServices;
using CodeArt.Util;

namespace CodeArt.Drawing.Imaging
{
    public static class ImageUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format">image.RawFormat的属性有时候无法正确的提取（例如从图像上clone出来的数据），因此需要手工指定</param>
        /// <returns></returns>
        public static byte[] GetBytes(this Bitmap image, ImageFormat format, int quality)
        {
            if (image == null) return Array.Empty<byte>();
            using (MemoryStream ms = new MemoryStream())
            {
                image.SaveAs(ms, format, quality);
                return ms.ToArray();
            }
        }

        //用此方法生成的字节数太大
        public static byte[] GetBytes(this Bitmap image)
        {
            var hasAlignedStride = image.Width % 4 != 0;
            var bd = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            var rawBitmapStride = hasAlignedStride ?
                    image.Width * Image.GetPixelFormatSize(bd.PixelFormat) / 8
                    : bd.Stride;

            var buffer = new byte[rawBitmapStride * image.Height];
            if (hasAlignedStride)
            {
                // bitmap data is 4-bytes aligned. Lets copy each scan line separately to get bitmap bytes compatible with ffmpeg rawvideo
                int j = bd.Height - 1;
                for (int h = 0; h < bd.Height; h++)
                {
                    IntPtr pointer = new IntPtr(bd.Scan0.ToInt64() + (bd.Stride * j));
                    Marshal.Copy(pointer, buffer, rawBitmapStride * (bd.Height - h - 1), rawBitmapStride);
                    j--;
                }
            }
            else
            {
                // just copy bitmap data (it is not 4-bytes aligned and compatible with ffmpeg rawvideo input)
                Marshal.Copy(bd.Scan0, buffer, 0, buffer.Length);
            }

            image.UnlockBits(bd);

            return buffer;
        }

        /// <summary>
        /// 深度拷贝位图，Bitmap.Clone()是浅拷贝
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Bitmap Copy(this Bitmap source)
        {
            Bitmap copy = new Bitmap(source.Width, source.Height);
            using (Graphics graphics = Graphics.FromImage(copy))
            {
                Rectangle imageRectangle = new Rectangle(0, 0, copy.Width, copy.Height);
                graphics.DrawImage(source, imageRectangle, imageRectangle, GraphicsUnit.Pixel);
            }
            return copy;
        }


        public static Bitmap GetBitmap(this byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                //return (Bitmap)Image.FromStream(ms);
                return new Bitmap(ms);
            }
        }


        public static Image ToImage(this Stream content)
        {
            return Image.FromStream(content);
        }

        /// <summary>
        /// 根据指定的宽高，等比缩放图片，多余的部分会以白边补全
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="quality">质量，这个参数并不太会影响图像的呈现质量，主要是可以提高速度,质量越低速度越快</param>
        /// <param name="background"></param>
        /// <returns></returns>
        public static Bitmap Scale(this Image source, int width, int height,ImageQuality quality, Color background)
        {
            var rawRect = new DynamicRectangle()
            {
                X = 0,
                Y = 0,
                Width = source.Width,
                Height = source.Height
            };

            var targetRect = rawRect.Scale(width, height, true);

            Bitmap target = null;
            try
            {
                target = new Bitmap(width, height);

                using (var gh = Graphics.FromImage(target))
                {
                    gh.CompositingQuality = quality.Compositing;
                    gh.SmoothingMode = quality.Smoothing;
                    gh.InterpolationMode = quality.Interpolation;
                    gh.PixelOffsetMode = quality.PixelOffset;
                    //gh.FillRectangle(new SolidBrush(background), 0, 0, width, height);

                    //清空画布并以指定的背景色填充
                    gh.Clear(background);

                    Rectangle destrect = targetRect.ToRectangle();
                    gh.DrawImage(source, destrect, 0, 0, source.Width,
                                    source.Height, GraphicsUnit.Pixel);
                }
            }
            catch (Exception ex)
            {
                if (target != null)
                    target.Dispose();
                throw ex;
            }
            return target;
        }

        public static Bitmap Cut(this Image source, Rectangle destrect, ImageQuality quality)
        {
            Bitmap target = null;
            try
            {
                target = new Bitmap(destrect.Width, destrect.Height);

                using (var gh = Graphics.FromImage(target))
                {
                    gh.CompositingQuality = quality.Compositing;
                    gh.SmoothingMode = quality.Smoothing;
                    gh.InterpolationMode = quality.Interpolation;
                    gh.PixelOffsetMode = quality.PixelOffset;

                    gh.DrawImage(source, destrect, 0, 0, destrect.Width,
                                    destrect.Height, GraphicsUnit.Pixel);

                    gh.DrawImage(source, new Rectangle(0, 0, destrect.Width, destrect.Height), destrect, GraphicsUnit.Pixel);
                }
            }
            catch (Exception ex)
            {
                if (target != null)
                    target.Dispose();
                throw ex;
            }
            return target;
        }


        /// <summary>
        /// 将此图像以指定的格式保存到指定的流中
        /// </summary>
        /// <param name="image"></param>
        /// <param name="target"></param>
        /// <param name="format"></param>
        /// <param name="quality">质量等级，0质量最差，体积最小，100质量最高，体积最大,小于0不压缩</param>
        public static void SaveAs(this Image image, Stream target, ImageFormat format, long quality = -1)
        {
            if (quality < 0) image.Save(target, format);
            else
            {
                if (quality > 100) quality = 100;
                var ici = GetEncoder(format);
                using (EncoderParameters ep = new EncoderParameters(1))
                {
                    ep.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                    image.Save(target, ici, ep);
                }
            }
        }


        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var mimeType = _getMimeType(format);
            if (string.IsNullOrEmpty(mimeType)) throw new DrawingException("暂不支持" + format.ToString() + "格式的图片");
            ImageCodecInfo ici = _getImageCodecInfo(mimeType);
            return ici;
        }


        private static Func<ImageFormat, string> _getMimeType = LazyIndexer.Init<ImageFormat, string>((format) =>
        {
            if (format == ImageFormat.Bmp) return "image/bmp";
            if (format == ImageFormat.Gif) return "image/gif";
            if (format == ImageFormat.Icon) return "image/x-icon";
            if (format == ImageFormat.Jpeg) return "image/jpeg";
            if (format == ImageFormat.Png) return "image/png";
            if (format == ImageFormat.Tiff) return "image/tiff";
            if (format == ImageFormat.Wmf) return "application/x-msmetafile";
            return string.Empty;
        });

        private static Func<string, ImageCodecInfo> _getImageCodecInfo = LazyIndexer.Init<string, ImageCodecInfo>((mimeType) =>
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        });

        //private static ImageCodecInfo GetEncoderInfo(string mimeType)
        //{
        //    int j;
        //    ImageCodecInfo[] encoders;
        //    encoders = ImageCodecInfo.GetImageEncoders();
        //    for (j = 0; j < encoders.Length; ++j)
        //    {
        //        if (encoders[j].MimeType == mimeType)
        //            return encoders[j];
        //    }
        //    return null;
        //}

        public static ImageFormat GetImageFormat(this string fileName)
        {
            var extension = PathUtil.GetExtension(fileName);
            switch (extension.ToLower())
            {
                case "png":
                case ".png": return ImageFormat.Png;
                case "jpg":
                case ".jpg":
                case "jpeg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case "gif": case ".gif": return ImageFormat.Gif;
            }
            throw new DrawingException("暂不支持" + extension + "的格式获取");
        }


        ///// <summary>
        ///// 图片压缩(降低质量以减小文件的大小)
        ///// </summary>
        ///// <param name="srcBitmap">传入的Bitmap对象</param>
        ///// <param name="destStream">压缩后的Stream对象</param>
        ///// <param name="level">压缩等级，0到100，0 最差质量，100 最佳</param>
        //public static void Compress(Bitmap srcBitmap, Stream destStream, long level)
        //{
        //    ImageCodecInfo myImageCodecInfo;
        //    Encoder myEncoder;
        //    EncoderParameter myEncoderParameter;
        //    EncoderParameters myEncoderParameters;

        //    // Get an ImageCodecInfo object that represents the JPEG codec.
        //    myImageCodecInfo = GetEncoderInfo("image/jpeg");

        //    // Create an Encoder object based on the GUID

        //    // for the Quality parameter category.
        //    myEncoder = Encoder.Quality;

        //    // Create an EncoderParameters object.
        //    // An EncoderParameters object has an array of EncoderParameter
        //    // objects. In this case, there is only one

        //    // EncoderParameter object in the array.
        //    myEncoderParameters = new EncoderParameters(1);

        //    // Save the bitmap as a JPEG file with 给定的 quality level
        //    myEncoderParameter = new EncoderParameter(myEncoder, level);
        //    myEncoderParameters.Param[0] = myEncoderParameter;
        //    srcBitmap.Save(destStream, myImageCodecInfo, myEncoderParameters);
        //}

    }
}
