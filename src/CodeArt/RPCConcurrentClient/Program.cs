using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.EasyMQ.RPC;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace RPCConcurrentClient
{
    class Program
    {
        static Program()
        {
            AppInitializer.Initialize();
        }

        static void Main(string[] args)
        {
            Task.Factory.StartNew(StartInvoke);
            Console.ReadLine();
        }


        private static void StartInvoke()
        {
            try
            {
                int max = 10;
                Console.WriteLine(string.Format("正在并发请求{0}次，如果服务器端的输出不是线性的，那么就是我们期望的结果", max));
                Parallel.For(0, max, (index) =>
                {
                    Console.WriteLine(string.Format("正在请求{0}次", index));
                    var result = RPCClient.Invoke("TestRPCConcurrent", (arg) =>
                    {
                        arg.Index = index;
                    });
                    Console.WriteLine(string.Format("第{0}次请求结束，返回值{1}", index, result.Info.Dynamic.Index));
                });
                Console.WriteLine("请求结束");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetCompleteMessage());
            }
        }


    }
}
