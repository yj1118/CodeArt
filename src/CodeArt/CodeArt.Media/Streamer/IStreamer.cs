using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace CodeArt.Media
{
    public interface IStreamer : IDisposable
    {
        /// <summary>
        /// 推流器推的流地址，该地址可以用于查看推流器推的流
        /// </summary>
        string FlowAddress { get; }

        /// <summary>
        /// 开始推流
        /// </summary>
        void Start();

        /// <summary>
        /// 结束推流
        /// </summary>
        void Stop();

    }
}
