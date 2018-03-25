using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Net.Anycast;

namespace CodeArt.Net.Anycast
{
    public interface IRtpModule
    {
        /// <summary>
        /// 安装模块
        /// </summary>
        /// <param name="context"></param>
        void Install(RtpCommunicator communicator);

        /// <summary>
        /// 移除模块
        /// </summary>
        /// <param name="context"></param>
        void Uninstall(RtpCommunicator communicator);
    }
}