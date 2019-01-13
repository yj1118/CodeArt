using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using CodeArt.IO;

namespace CodeArt.Media
{
    public class GPUEncoderSharer : IDisposable
    {

        #region  工厂构造


        public static GPUEncoderSharer CreateSharer(string identity)
        {
            var streamer = new GPUEncoderStreamer(identity);
            return new GPUEncoderSharer(streamer);
        }

        #endregion

        private GPUEncoderStreamer _streamer;

        private GPUEncoderSharer(GPUEncoderStreamer streamer)
        {
            _streamer = streamer;
        }

        public void Start()
        {
            _streamer.Start();
        }

        public void Stop()
        {
            _streamer.Stop();
        }

        public void Dispose()
        {
            _streamer.Dispose();
        }

    }
}
