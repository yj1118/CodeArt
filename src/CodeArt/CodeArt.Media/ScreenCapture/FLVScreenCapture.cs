
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Accord.Video.FFMPEG;
using System.Diagnostics;
using CodeArt.Drawing.Imaging;
using CodeArt.IO;

using CodeArt.Media.Converter;

using CodeArt.Log;

namespace CodeArt.Media
{
    public class FLVScreenCapture : IScreenCapture,IDisposable
    {
        private FFMpegConverter _converter;
        private ConvertSettings _settings;

        private string _saveFileName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution">请使用FrameSize常量</param>
        /// <param name="maxDuration"></param>
        /// <param name="quality">画质，值越小画质越好、体积越大，0的画质最高，但是体积也最大</param>
        public FLVScreenCapture(string resolution, float maxDuration, int quality, string audioDevice, string saveFileName)
        {
            _converter = new FFMpegConverter();
            _settings = new ConvertSettings();

            if (maxDuration > 0) _settings.MaxDuration = maxDuration;

            _settings.VideoFrameSize = resolution;
            _settings.VideoFrameRate = 25;
            _settings.AudioDevice = audioDevice;
            _settings.CustomInputArgs = " -draw_mouse 1 ";
            _settings.CustomOutputArgs = " -qscale " + quality;

            _saveFileName = saveFileName;
        }

        private void InitSaveFile()
        {
            IOUtil.CreateFileDirectory(_saveFileName);
        }

        private void DisposeSaveFile()
        {
            try
            {
                IOUtil.Delete(_saveFileName);
            }
            catch (Exception ex)
            {
                //写日志todo...
            }
        }


        public void Start()
        {
            InitSaveFile();

            var t = Task.Run(() => {
                try
                {
                    _converter.ConvertMedia("desktop", "gdigrab", _saveFileName, null, _settings);
                }
                catch (Exception ex)
                {
                    LogWrapper.Default.Fatal(ex);
                }
            });
        }

        public void Stop()
        {
            AbortConverter();
            DisposeSaveFile();
        }

        private void AbortConverter()
        {
            if (!_converter.Stop())
            {
                _converter.Abort();
            }

            _converter.Dispose();
        }


        public void Dispose()
        {
            AbortConverter();
            DisposeSaveFile();
        }
    }

    

}
