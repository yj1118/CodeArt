using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.TestTools.DomainDriven;

namespace DomainEventApp0
{
    class Program
    {
        static void Main(string[] args)
        {
            AppInitializer.Initialize();

            var cmd = new StartEventTest(_simple);
            cmd.Execute();


            Console.WriteLine("按任意键结束");
            Console.ReadLine();

            AppInitializer.Cleanup();
        }

        private static string _simple = "{raise:1,pre:[{raise:1},{raise:1}]}";

        private string GetMessage(string config)
        {
            return config;//目前简单处理
        }

    }
}
