using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.EasyMQ.RPC;
using CodeArt.DTO;
using CodeArt.AppSetting;

namespace ServerApp3
{
    class Program
    {
        static Program()
        {
            AppInitializer.Initialize();
        }

        static void Main(string[] args)
        {
            RPCServer.Open("getName2", new Handler());

            Console.WriteLine("已启动getName2的服务，按任意键退出");
            Console.ReadLine();
            RPCServer.Close("getName2");
        }



        private class Handler : IRPCHandler
        {
            public DTObject Process(string method, DTObject args)
            {
                Console.WriteLine(string.Format("调用方法:{0},参数:{1}", method, args.GetCode()));
                var result = DTObject.CreateReusable();
                result["value"] = 1;
                return result;
            }
        }



    }
}