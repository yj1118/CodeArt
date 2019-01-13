using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.ServiceModel;
using CodeArt.DTO;
using CodeArt.AppSetting;

namespace ServiceModelMQTestClient
{
    class Program
    {
        static Program()
        {
            AppInitializer.Initialize();
        }


        static void Main(string[] args)
        {
            var fileName0 = @"D:\Workspace\Projects\CodeArt Framework\CodeArt\CodeArtTest\DTO\code.txt";
            var sourceCode0 = System.IO.File.ReadAllText(fileName0);

            Parallel.For(0, 1000, (index) =>
            {
                Console.WriteLine("index:" + index);
                {
                    var arg = DTObject.Create(sourceCode0);
                    var result = ServiceContext.Invoke("CreateObject", arg);
                    var resultCode = result.GetCode(false, false);
                    var argCode = arg.GetCode();
                    if (resultCode != argCode)
                    {
                        Console.WriteLine("1");
                    }
                }
            });

            Console.WriteLine("执行完毕");
        }
    }
}
