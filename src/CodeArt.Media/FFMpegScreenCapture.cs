
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using CodeArt.Drawing.Imaging;
using CodeArt.IO;


using NReco.VideoConverter;
using Accord.Video.FFMPEG;
using System.Diagnostics;
using CodeArt.Log;

namespace CodeArt.Media
{
    public class FFMpegScreenCapture
    {
        private FFMpegConverter _converter;
        private ConvertSettings _settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution">请使用FrameSize常量</param>
        /// <param name="maxDuration"></param>
        public FFMpegScreenCapture(string resolution, float maxDuration)
        {
            _converter = new FFMpegConverter();
            _settings = new ConvertSettings();
            if (maxDuration > 0) _settings.MaxDuration = maxDuration;
            _settings.VideoFrameSize = resolution;
            _settings.VideoFrameRate = 30;
            _settings.CustomInputArgs = " -draw_mouse 1 "; //绘制鼠标
        }

        /// <summary>
        /// 采集一次
        /// </summary>
        /// <returns></returns>
        public byte[] Start()
        {
            byte[] data = Array.Empty<byte>();
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    _converter.ConvertMedia(
                        "desktop",
                        "gdigrab",
                        ms,
                        "mp4",
                        _settings
                    );
                    data = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                LogWrapper.Default.Fatal(ex);
            }
            return data;
        }

        public void Dispose()
        {
            if (!_converter.Stop())
            {
                _converter.Abort();
            }
        }

     
        //#region 静态方法

        //public static Bitmap[] GetBitmaps(byte[] data)
        //{
        //    Bitmap[] frames = null;
        //    using (MemoryStream input = new MemoryStream(data))
        //    {
        //        var ff = new FFMpegConverter();
        //        using (var output = new BitmapStream(1280, 720))
        //        {
        //            var task = ff.ConvertLiveMedia(
        //                input, // 从内存流中读取
        //                null,  // 自动从流中检测格式
        //                output,
        //                "rawvideo",
        //                new ConvertSettings()
        //                {
        //                    VideoFrameSize = String.Format("{0}x{1}", 1280, 720),  // 可以设置输出图像的大小
        //                    CustomOutputArgs = " -pix_fmt gdigrab ", // windows bitmap pixel format
        //                    VideoFrameRate = 25
        //                });


        //            task.Start();
        //            task.Wait(); // use task.wait if input is provided with ff.Write method
        //            frames = output.GetBitmaps();
        //        }
        //    }
        //    return frames;
        //}

        //#endregion


    }
}
