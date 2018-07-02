
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
    public struct FLVScreenCaptureConfig
    {
        public string Resolution
        {
            get;
            private set;
        }

        public float MaxDuration
        {
            get;
            private set;
        }

        public int Quality
        {
            get;
            private set;
        }

        public string AudioDevice
        {
            get;
            private set;
        }

        public FLVScreenCaptureConfig(float maxDuration, int quality, string audioDevice, string resolution = FrameSize.hd1080)
        {
            this.MaxDuration = maxDuration;
            this.Quality = quality;
            this.AudioDevice = audioDevice;
            this.Resolution = resolution;
        }

    }

}
