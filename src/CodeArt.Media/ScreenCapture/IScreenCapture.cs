
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

using CodeArt.Media.Converter;
using Accord.Video.FFMPEG;
using System.Diagnostics;
using CodeArt.Log;

namespace CodeArt.Media
{
    public interface IScreenCapture : IDisposable
    {

        /// <summary>
        /// 开始捕捉屏幕
        /// </summary>
        void Start();

        /// <summary>
        /// 停止捕捉
        /// </summary>
        void Stop();
    }

}
