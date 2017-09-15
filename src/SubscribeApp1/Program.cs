using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.EasyMQ.Event;
using CodeArt.Concurrent;
using CodeArt.AppSetting;

using EventHandler = CodeArt.EasyMQ.Event.EventHandler;

namespace SubscribeApp
{
    class Program
    {
        static Program()
        {
            AppInitializer.Initialize();
        }


        static void Main(string[] args)
        {
            EventPortal.Subscribe("Event1",new Handler1());
            EventPortal.Subscribe("Event1",new Handler2());

            Console.WriteLine("group0 已订阅事件Event1，按任意键退出程序......");
            Console.ReadLine();

            EventPortal.Cancel("Event1");
        }

        [SafeAccess]
        private class Handler1 : EventHandler
        {
            protected override void DynamicHandle(dynamic @event)
            {
                Console.WriteLine(string.Format("[handler1]name:{0},id:{1}", @event.Name, @event.Id));
            }
        }


        [SafeAccess]
        private class Handler2 : EventHandler
        {
            protected override void DynamicHandle(dynamic @event)
            {
                Console.WriteLine(string.Format("[handler2]name:{0},id:{1}", @event.Name, @event.Id));
            }
        }


    }
}
