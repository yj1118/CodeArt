using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;
using CodeArt.TestTools.DomainDriven;

namespace DomainEventApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            AppInitializer.Initialize();

            Console.WriteLine("按任意键结束");
            Console.ReadLine();


            AppInitializer.Cleanup();
        }


    }
}
