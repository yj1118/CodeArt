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
        void Open(IRPCHandler handler);

        void Close();
    }
}