using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CodeArt.IO;

namespace CodeArt.Media
{
    public class ScreenSharer : IDisposable
    {
        static ScreenSharer()
        {
            try
            {
                IOUtil.ClearDirectory(FLVTempFolder);
            }
            catch(Exception ex)
            {
                //写日志todo...
            }
        }

        #region  工厂构造

        private static string FLVTempFolder = Path.Combine(Path.GetTempPath(), "ScreenSharer", "flv");


        public static ScreenSharer CreateFileRtmp(FLVScreenCaptureConfig captureConfig, RtmpStreamerConfig streamerConfig)
        {
            string tempFileName = Path.Combine(FLVTempFolder, string.Format("{0}.flv", Guid.NewGuid()));
            var capture = new FLVScreenCapture(captureConfig.Resolution, captureConfig.MaxDuration, captureConfig.Quality, captureConfig.AudioDevice, tempFileName);
            var streamer = new RtmpStreamer(streamerConfig.ServerIP, streamerConfig.ServerPort, streamerConfig.Identity, tempFileName);
            return new ScreenSharer(capture, streamer);
        }

        #endregion

        private IStreamer _streamer;
        private IScreenCapture _capture;
        public string FlowAddress { get; private set; }

        private ScreenSharer(IScreenCapture capture, IStreamer streamer)
        {
            _capture = capture;
            _streamer = streamer;
        }

        public void Start()
        {
            _capture.Start();
            _streamer.Start();
            this.FlowAddress = _streamer.FlowAddress;
        }

        public void Stop()
        {
            _streamer.Stop();
            _capture.Stop();
            this.FlowAddress = null;
        }

        public void Dispose()
        {
            _streamer.Dispose();
            _capture.Dispose();
        }

    }
}
