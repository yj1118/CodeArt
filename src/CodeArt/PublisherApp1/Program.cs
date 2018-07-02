using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using CodeArt.AppSetting;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.EasyMQ.Event;

namespace PublisherApp1
{
    class Program
    {
        static Program()
        {
            AppInitializer.Initialize();
        }


        static void Main(string[] args)
        {
            Task.Factory.StartNew(StartPublish);
            Console.WriteLine("按任意键可以退出程序");
            Console.ReadLine();
        }

        private static void StartPublish()
        {
            int count = 0;
            while(true)
            {
                count++;
                var evt = GetEvent(count);
                EventPortal.Publish(EventName, evt);
                Console.WriteLine(string.Format("已发布事件{0},id:{1}", EventName, count));
                Thread.Sleep(2000);
            }
        }

        const string EventName = "Event1";

        private static DTObject GetEvent(int id)
        {
            var dto = DTObject.Create();
            dto["name"] = EventName;
            dto["id"] = id;
            return dto;
        }

    }
}
