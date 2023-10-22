using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.AppSetting;

namespace CodeArt.EasyMQ.RPC
{
    public interface IServer
    {
        /// <summary>
        /// 服务的名称
        /// </summary>
        string Name { get; }

        void Initialize(IRPCHandler handler);

        void Open();

        void Close();
    }
}