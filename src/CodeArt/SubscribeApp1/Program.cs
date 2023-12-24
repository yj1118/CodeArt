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
using CodeArt.EasyMQ;

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
            public override void Handle(string eventName, TransferData arg)
            {
                Console.WriteLine(string.Format("[handler1]name:{0},id:{1}", arg.Info.Dynamic.Name, arg.Info.Dynamic.Id));
            }
        }


        [SafeAccess]
        private class Handler2 : EventHandler
        {
            public override void Handle(string eventName, TransferData arg)
            {
                Console.WriteLine(string.Format("[handler2]name:{0},id:{1}", arg.Info.Dynamic.Name, arg.Info.Dynamic.Id));
            }
        }


    }
}
