using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace CodeArt.Media
{
    /// <summary>
    /// 基于rtmp协议的推流器
    /// </summary>
    public class RtmpStreamer : IStreamer,IDisposable
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

        public string FLVName
        {
            get;
            private set;
        }


        public RtmpStreamer(string serverIP, int serverPort, string identity, string flvName)
        {
            this.ServerIP = serverIP;
            this.ServerPort = serverPort;
            this.Identity = identity;
            this.FLVName = flvName;
        }

        private Process _process;
        public string FlowAddress
        {
            get;
            private set;
        }

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

        private void InitializeProcess()
        {
            _process = new Process();
            _process.StartInfo.FileName = StreamerFileName;

            // 推流地址
            this.FlowAddress = string.Format("rtmp://{0}:{1}/live/{2}", this.ServerIP, this.ServerPort, this.Identity);

            // 启动参数设置
            _process.StartInfo.Arguments = string.Format("{0} {1}", this.FLVName, this.FlowAddress);

            // 启动方式设置
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
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
                _process.Kill();
                _process.Close();
                _process = null;
                this.FlowAddress = null;
            }
        }

        public void Dispose()
        {
            DisposeProcess();
        }


        private static string StreamerFileName = string.Format("{0}_Media\\Streamer\\rtmpstreamer.exe", AppDomain.CurrentDomain.BaseDirectory);

        static RtmpStreamer()
        {
            // 检查推流器是否存在
            if (!File.Exists(StreamerFileName))
                throw new RtmpStreamerNotExist();
        }

        /// <summary>
        /// 清理运行环境
        /// </summary>
        public static void CleanUp()
        {
            Process[] processes = Process.GetProcessesByName("rtmpstreamer");
            foreach (Process p in processes)
            {
                p.Kill();
                p.Close();
            }
        }
    }
}
