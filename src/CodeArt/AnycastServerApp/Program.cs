using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Net.Anycast;

namespace AnycastServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerEvents.Error += OnError;
            ServerEvents.Running += OnRunning;
            ServerEvents.ClientConnected += OnClientConnected;
            ServerEvents.ClientDisconnected += OnClientDisconnected;
            ServerEvents.HeartBeatReceived += OnHeartBeatReceived;

            ServerConfig config = new ServerConfig();

            var server = new AnycastServer(config);
            var task = server.Run();

            Console.ReadKey();

            task = server.Stop();
            task.Wait();
        }

        private static void OnHeartBeatReceived(object sender, ServerEvents.HeartBeatReceivedEventArgs ea)
        {
            Console.WriteLine("收到用户心跳包:" + DateTime.Now.ToString("mm:ss") + " - " + ea.Session.UnicastAddress);
        }

        private static void OnClientDisconnected(object sender, ServerEvents.ClientDisconnectedEventArgs ea)
        {
            Console.WriteLine("用户已离开:" + ea.Session.UnicastAddress);
        }

        private static void OnClientConnected(object sender, ServerEvents.ClientConnectedEventArgs ea)
        {
            Console.WriteLine("用户已加入:" + ea.Session.UnicastAddress);
        }

        private static void OnRunning(object sender, ServerEvents.RunningEventArgs ea)
        {
            Console.WriteLine("服务器已启动,按任意键可以关闭");
        }

        private static void OnError(object sender, AnycastEventsBase.ErrorEventArgs ea)
        {
            Console.WriteLine("错误信息" + ea.Exception.ToString());
        }
    }
}
