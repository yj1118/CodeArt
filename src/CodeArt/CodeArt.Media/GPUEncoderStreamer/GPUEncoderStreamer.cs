using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;


namespace CodeArt.Media
{

    public class GPUEncoderStreamer : IDisposable
    {
        /// <summary>
        /// 推流器的身份，接收端用该身份来查看需要的流
        /// </summary>
        public string Identity
        {
            get;
            private set;
        }

        public GPUEncoderStreamer(string identity)
        {
            this.Identity = identity;
        }

        private Process _process;
        private static string EncoderName = string.Format("{0}_Media\\GPUEncoder\\GPUEncoder.exe", AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// 开始推流
        /// </summary>
        public void Start()
        {
            InitializeProcess();
            _process.Start();
        }

        private void InitializeProcess()
        {
            _process = new Process();
            _process.StartInfo.FileName = EncoderName;
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


        /// <summary>
        /// 清理运行环境
        /// </summary>
        public static void CleanUp()
        {
            Process[] processes = Process.GetProcessesByName("GPUEncoder");
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
