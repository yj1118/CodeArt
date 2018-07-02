using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;


namespace CodeArt.EasyMQ.RPC
{
    /// <summary>
    /// 事件的订阅者
    /// </summary>
    public interface IClient
    {
        DTObject Invoke(string method, DTObject arg);

        /// <summary>
        /// 清理客户端资源
        /// </summary>
        void Clear();

    }
}
