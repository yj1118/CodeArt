using CodeArt.EasyMQ.Event;
using CodeArt.EasyMQ.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.TestPlatform
{
    public static class ServiceUtil
    {
        const string ClearBufferMethod = "d:ClearBuffer";
        const string ClearDataMethod = "d:ClearData";

        /// <summary>
        /// 清空服务端领域缓冲区
        /// </summary>
        public static void ClearBuffer()
        {
            DTObject arg = DTObject.Empty;
            EventPortal.Publish(ClearBufferMethod, arg);
        }

        public static void ClearData()
        {
            DTObject arg = DTObject.Empty;
            EventPortal.Publish(ClearDataMethod, arg);
        }

    }
}
