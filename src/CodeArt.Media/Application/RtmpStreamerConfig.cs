
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
    public struct RtmpStreamerConfig
    {
        public string ServerIP
        {
            get;
            private set;
        }

        public int ServerPort
        {
            get;
            private set;
        }

        public string Identity
        {
            get;
            private set;
        }

        public RtmpStreamerConfig(string serverIP, int serverPort, string identity)
        {
            this.ServerIP = serverIP;
            this.ServerPort = serverPort;
            this.Identity = identity;
        }

    }

}
