
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
    //[Obsolete("性能完全不行，找到原因并解決后再考慮使用")]
    public class FFMpegObsolete
    {
        //public static byte[] ConvertVideo(Bitmap[] frames, int frameRate, string videoFormat = Format.h264)
        //{
        //    if (frames.Length == 0) throw new ArgumentOutOfRangeException("生成视频文件的图片数不能为0");
        //    var first = frames[0];
        //    ValidatePixelFormat(first.PixelFormat);

        //    var width = first.Width;
        //    var height = first.Height;

        //    var ff = new FFMpegConverter();
        //    using (var output = new MemoryStream())
        //    {
        //        var task = ff.ConvertLiveMedia(
        //                    "rawvideo",
        //                    output,
        //                    videoFormat,
        //                    new ConvertSettings()
        //                    {
        //                        // bgr24 = windows bitmap pixel format
        //                        // framerate = set frame rate of _input_ (in this example 5 = 5 bitmaps for 1 second of the video)
        //                        CustomInputArgs = string.Format(" -pix_fmt bgr24 -video_size {0}x{1} -framerate {2} ",
        //                            width, height, frameRate)
        //                    });

        //        task.Start();

        //        foreach (var frame in frames)
        //        {
        //            task.Write(frame);
        //        }

        //        // wait until convert is finished and stop live streaming
        //        task.Stop();

        //        //var data = output.ToArray();
        //        //using (var h264stream = new FileStream("out.h264", FileMode.Create, FileAccess.Write))
        //        //{
        //        //    h264stream.Write(data, 0, data.Length);
        //        //    ff.ConvertMedia("out.h264", "h264", "out.avi", null, new ConvertSettings());
        //        //}
        //        //return data;

        //        return output.ToArray();
        //    }
        //}

        public static Bitmap[] ConvertVideo(byte[] data, int frameRate, int outputWidth, int outputHeight)
        {
            Bitmap[] frames = null;
            using (MemoryStream input = new MemoryStream(data))
            {
                var ff = new FFMpegConverter();
                using (var output = new BitmapStream(outputWidth, outputHeight))
                {
                    var task = ff.ConvertLiveMedia(
                        input, // 从内存流中读取
                        null,  // 自动从流中检测格式
                        output,
                        "rawvideo",
                        new ConvertSettings()
                        {
                            VideoFrameSize = String.Format("{0}x{1}", outputWidth, outputHeight),  // 可以设置输出图像的大小
                            CustomOutputArgs = " -pix_fmt bgr24 ", // windows bitmap pixel format
                            VideoFrameRate = frameRate
                        });


                    task.Start();
                    task.Wait(); // use task.wait if input is provided with ff.Write method
                    frames = output.GetBitmaps();
                }
            }
            return frames;
        }

        static FFMpegObsolete()
        {
            var temporary = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporary Files");
            Directory.CreateDirectory(temporary);
            IOUtil.ClearDirectory(temporary);
        }
    }
}
