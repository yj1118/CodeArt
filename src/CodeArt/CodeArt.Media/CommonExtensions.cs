using System;
using System.Linq;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Threading;
using CodeArt.Log;
using System.Collections.Concurrent;
using NReco.VideoConverter;
using System.Drawing;
using CodeArt.Drawing.Imaging;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace CodeArt.Media
{
    /// <summary>
    /// 常用的扩展
    /// </summary>
    public static class CommonExtensions
    {
        public static void Write(this ConvertLiveMediaTask task, Bitmap frame)
        {
            var buffer = frame.GetBytes();
            task.Write(buffer, 0, buffer.Length);
        }
    }
}
