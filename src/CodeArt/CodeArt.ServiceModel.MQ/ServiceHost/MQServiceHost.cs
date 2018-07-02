using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;

using CodeArt;
using CodeArt.DTO;
using CodeArt.Log;
using CodeArt.Concurrent;
using CodeArt.EasyMQ.RPC;
using CodeArt.AppSetting;
using CodeArt.Util;

namespace CodeArt.ServiceModel
{
    public static class MQServiceHost
    {

        public static void Start(Action initialize = null)
        {
            RPCEvents.ServerOpened += OnServerOpened;
            RPCEvents.ServerError += OnServerError;
            RPCEvents.ServerClosed += OnServerClosed;

            AppInitializer.Initialize();

            if (initialize != null)
                initialize();

            Console.WriteLine(MQ.Strings.CloseServiceHost);
            Console.ReadLine();


            AppInitializer.Cleanup();
        }

        private static void OnServerError(object sender, RPCEvents.ServerErrorArgs arg)
        {
            Console.WriteLine(arg.Exception.GetCompleteMessage());
        }

        private static void OnServerOpened(object sender, RPCEvents.ServerOpenedArgs arg)
        {
            Console.WriteLine(string.Format(MQ.Strings.ServiceIsOpen, arg.MethodName));
        }

        private static void OnServerClosed(object sender, RPCEvents.ServerClosedArgs arg)
        {
            Console.WriteLine(string.Format(MQ.Strings.ServiceIsClose, arg.MethodName));
        }

    }
}
