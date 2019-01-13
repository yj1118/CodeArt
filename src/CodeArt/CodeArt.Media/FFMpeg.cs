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
using CodeArt.Log;
using System.Diagnostics;



using NReco.VideoConverter;
using Accord.Video.FFMPEG;

namespace CodeArt.Media
{
    public class FFMpeg
    {
        //private static object _syncObject = new object();

        //public static byte[] ConvertVideo(Bitmap[] frames, int frameRate)
        //{
        //    if (frames.Length == 0) throw new ArgumentOutOfRangeException("生成视频文件的图片数不能为0");
        //    var first = frames[0];

        //    var width = first.Width;
        //    var height = first.Height;

        //    var id = Guid.NewGuid().ToString();
        //    var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporary Files", string.Format("{0}.avi", id));

        //    byte[] data = Array.Empty<byte>();
        //    try
        //    {
        //        lock(_syncObject)
        //        {
        //            using (var writer = new VideoFileWriter())
        //            {
        //                writer.Open(fileName, width, height, frameRate, VideoCodec.H264);
        //                foreach (var frame in frames)
        //                {
        //                    writer.WriteVideoFrame(frame);
        //                }
        //            }
        //        }


        //        const int bufferSize = 128 * 104;
        //        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        //        {
        //            data = fs.GetBytes(bufferSize);
        //        }
        //        File.Delete(fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWrapper.Default.Fatal(ex);
        //    }
        //    return data;
        //}

        //public static Bitmap[] ConvertVideo(byte[] data)
        //{
        //    var id = Guid.NewGuid().ToString();
        //    var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporary Files", string.Format("{0}.avi", id));

        //    using (FileStream fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
        //    {
        //        fs.Write(data, 0, data.Length);
        //    }

        //    Bitmap[] images = Array.Empty<Bitmap>();
        //    try
        //    {
        //        lock (_syncObject)
        //        {
        //            using (var reader = new VideoFileReader())
        //            {
        //                reader.Open(fileName);
        //                images = new Bitmap[reader.FrameCount];
        //                for (var i = 0; i < images.Length; i++)
        //                {
        //                    images[i] = reader.ReadVideoFrame();
        //                }
        //            }
        //        }

        //        File.Delete(fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogWrapper.Default.Fatal(ex);
        //    }
        //    return images;
        //}

        ////public static byte[] Compress(Bitmap frame)
        ////{
        ////    var width = frame.Width;
        ////    var height = frame.Height;

        ////    var id = Guid.NewGuid().ToString();
        ////    var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporary Files", string.Format("{0}.avi", id));

        ////    lock (_syncObject)
        ////    {
        ////        using (var writer = new VideoFileWriter())
        ////        {
        ////            writer.Open(fileName, width, height, 1, VideoCodec.H264);
        ////            writer.WriteVideoFrame(frame);
        ////        }
        ////    }

        ////    byte[] data = null;
        ////    const int bufferSize = 128 * 104;
        ////    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        ////    {
        ////        data = fs.GetBytes(bufferSize);
        ////    }
        ////    File.Delete(fileName);
        ////    return data;
        ////}

        ////public static Bitmap Decompress(byte[] data)
        ////{
        ////    var id = Guid.NewGuid().ToString();
        ////    var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporary Files", string.Format("{0}.avi", id));

        ////    using (FileStream fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
        ////    {
        ////        fs.Write(data, 0, data.Length);
        ////    }

        ////    Bitmap frame = null;
        ////    using (var reader = new VideoFileReader())
        ////    {
        ////        reader.Open(fileName);
        ////        frame = reader.ReadVideoFrame();
        ////    }
        ////    File.Delete(fileName);
        ////    return frame;
        ////}

        //static FFMpeg()
        //{
        //    var temporary = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temporary Files");
        //    Directory.CreateDirectory(temporary);
        //    IOUtil.ClearDirectory(temporary);
        //}

        /// <summary>
        /// 清理运行环境
        /// </summary>
        public static void CleanUp()
        {
            Process[] processes = Process.GetProcessesByName("ffmpeg");
            foreach (Process p in processes)
            {
                try
                {
                    p.Kill();
                    p.Close();
                }
                catch
                {

                }
            }
        }


    }
}
