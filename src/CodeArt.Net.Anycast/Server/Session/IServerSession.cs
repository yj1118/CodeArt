using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DotNetty.Transport.Channels;

namespace CodeArt.Net.Anycast
{
    public interface IServerSession
    {
        /// <summary>
        /// 会话的单播地址，通过该地址可以找到会话
        /// </summary>
        string UnicastAddress { get; }

        /// <summary>
        /// 回话关闭
        /// </summary>
        void Close();

        /// <summary>
        /// 是否为活动的
        /// </summary>
        /// <returns></returns>
        bool IsActive { get; }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg"></param>
        void Process(Message msg);
    }
}
