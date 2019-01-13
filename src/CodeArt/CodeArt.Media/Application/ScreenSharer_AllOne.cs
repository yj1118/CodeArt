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

        #region  工厂构造


        public static ScreenSharer CreateFileRtmp(string serverIP, int serverPort, string identity, string resolution, int quality, string audioDevice)
        {
            var streamer = new ScreenStreamer(serverIP, serverPort, identity, resolution, quality, audioDevice);
            return new ScreenSharer(streamer);
        }

        #endregion

        private ScreenStreamer _streamer;

        public string FlowAddress { get; private set; }

        private ScreenSharer(ScreenStreamer streamer)
        {
            _streamer = streamer;
        }

        public void Start()
        {
            _streamer.Start();
            this.FlowAddress = _streamer.FlowAddress;
        }

        public void Stop()
        {
            _streamer.Stop();
            this.FlowAddress = null;
        }

        public void Dispose()
        {
            _streamer.Dispose();
        }

    }
}
