using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;


namespace CodeArt.EasyMQ.RPC
{
    /// <summary>
    /// 
    /// </summary>
    public interface IClient
    {
        TransferData Invoke(string method, DTObject arg);

        /// <summary>
        /// 清理客户端资源
        /// </summary>
        void Clear();

    }
}
