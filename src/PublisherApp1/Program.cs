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
                var evt = new Event1(count);
                EventPortal.Publish(evt);
                Console.WriteLine(string.Format("已发布事件{0},id:{1}", typeof(Event1).Name, count));
                Thread.Sleep(2000);
                break;
            }
        }



        [SafeAccess]
        private class Event1 : EventBase
        {
            private int _id;

            public Event1(int id)
                : base("Event1")
            {
                _id = id;
            }

            public override DTObject GetRemotable()
            {
                var dto = DTObject.Create();
                dto["name"] = this.Name;
                dto["id"] = _id;
                return dto;
            }

        }

    }
}
