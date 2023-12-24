using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using CodeArt.EasyMQ.RPC;
using CodeArt.DTO;
using CodeArt.AppSetting;
using CodeArt.Log;
using CodeArt.EasyMQ;

namespace RPCConcurrentServer
{
    class Program
    {
        static Program()
        {
            AppInitializer.Initialize();
        }

        static void Main(string[] args)
        {
            RPCServer.Open("TestRPCConcurrent", new Handler());

            Console.WriteLine("已启动TestRPCConcurrent的服务，按任意键退出");
            Console.ReadLine();
            RPCServer.Close("TestRPCConcurrent");
        }

        private class Handler : IRPCHandler
        {
            public TransferData Process(string method, DTObject args)
            {
                int current = Interlocked.Increment(ref count);

                Console.WriteLine(string.Format("开始处理第{0}次调用请求", current));

                Thread.Sleep(2000); //模拟处理2秒

                Console.WriteLine(string.Format("结束处理第{0}次调用请求", current));

                var dto = DTObject.Create();
                dto.Dynamic.Index = args.Dynamic.Index;

                return new TransferData(dto);
            }
        }

        private static int count = 0;

    }
}