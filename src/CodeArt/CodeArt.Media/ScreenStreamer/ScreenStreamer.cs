using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;


namespace CodeArt.Media
{

    public class ScreenStreamer : IDisposable
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

        /// <summary>
        /// 推流器的身份，接收端用该身份来查看需要的流
        /// </summary>
        public string Identity
        {
            get;
            private set;
        }

        private int _quality;
        private bool _drawMouse;
        private string _resolution;
        private string _audioDevice;
        private int _rate;


        public ScreenStreamer(string serverIP, int serverPort, string identity, string resolution, int quality, string audioDevice)
        {
            this.ServerIP = serverIP;
            this.ServerPort = serverPort;
            this.Identity = identity;

            _resolution = resolution;
            _rate = 15;
            _audioDevice = audioDevice;
            _drawMouse = true;
            _quality = quality;
        }

        private Process _process;


        /// <summary>
        /// 开始推流
        /// </summary>
        /// <param name="flvFilePath"></param>
        /// <returns></returns>
        public void Start()
        {
            InitializeProcess();
            _process.Start();
        }

        public string FlowAddress
        {
            get;
            private set;
        }

        private void InitializeProcess()
        {
            _process = new Process();
            _process.StartInfo.FileName = FFMPEGFileName;

            this.FlowAddress = string.Format("rtmp://{0}:{1}/live/{2}", this.ServerIP, this.ServerPort, this.Identity);

            // 启动参数设置
            _process.StartInfo.Arguments = GetArgumentsCode();

            // 启动方式设置
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
        }

        private string GetArgumentsCode()
        {
            //return string.Format("-f gdigrab -i desktop -r 30 -s hd1080 -qscale 15 -f flv {0}", this.FlowAddress);
            //return string.Format("-f gdigrab -i desktop -r 30 -s hd1080  -qscale 15 -f flv {0}",this.FlowAddress);
            //return string.Format("-f dshow -i audio=\"麦克风 (Realtek High Definition Audio)\" -f gdigrab -draw_mouse 1 -i desktop -r 30 -s hd1080 -qscale 15 -f flv {0}", this.FlowAddress);

            StringBuilder code = new StringBuilder();

            if (!string.IsNullOrEmpty(_audioDevice))
            {
                code.AppendFormat("-f dshow -i audio=\"{0}\" ", _audioDevice);
            }
            code.Append("-f gdigrab ");
            if (_drawMouse)
            {
                code.Append("-draw_mouse 1 ");
            }
            code.Append("-i desktop ");
            code.AppendFormat("-r {0} ", _rate);

            code.AppendFormat("-s {0} -qscale {1} -preset:v ultrafast -tune:v zerolatency -f flv ", _resolution, _quality);
            code.Append(this.FlowAddress);
            return code.ToString();
        }


        /// <summary>
        /// 停止推流
        /// </summary>
        public void Stop()
        {
            DisposeProcess();
        }

        private void DisposeProcess()
        {
            if (_process != null)
            {
                try
                {
                    _process.Kill();
                    _process.Close();
                    _process = null;
                }
                catch
                {

                }
            }
        }

        public void Dispose()
        {
            DisposeProcess();
        }

        private static string FFMPEGFileName = string.Format("{0}_Media\\ffmpeg.exe", AppDomain.CurrentDomain.BaseDirectory);

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
