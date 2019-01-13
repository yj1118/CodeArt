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

namespace ClientApp1
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
                int count = 0;
                while (true)
                {
                    count++;
                    Console.WriteLine("正在请求方法getName1,参数" + count);
                    var result1 = RPCClient.Invoke("getName1", (arg) =>
                    {
                        arg["client1-count"] = count;
                    });
                    Console.WriteLine("收到请求结果：" + result1.Info.GetCode());
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetCompleteMessage());
            }
        }


    }
}
