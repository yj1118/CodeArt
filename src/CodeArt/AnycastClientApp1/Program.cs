using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Net.Anycast;

namespace AnycastClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientEvents.Connecting += OnConnecting;
            ClientEvents.Error += OnError;
            ClientEvents.Connected += OnConnected;
            ClientEvents.HeartBeatReceived += OnHeartBeatReceived;
            ClientEvents.Disconnected += OnDisconnected;

            ClientConfig config = new ClientConfig();
            var client = new AnycastClient(config);
            client.Connect(false);

            Console.ReadKey();

            client.Dispose();
        }

        private static void OnHeartBeatReceived(object sender, ClientEvents.HeartBeatReceivedEventArgs ea)
        {
            Console.WriteLine("收到心跳包:" + DateTime.Now.ToString("mm:ss") + " - " + ea.ServerEndPoint.ToString());
        }

        private static void OnError(object sender, AnycastEventsBase.ErrorEventArgs ea)
        {
            if(ea.Exception is ConnectServerException)
            {
                Console.WriteLine("连接服务器失败");
                return;
            }

            if(ea.Exception is ReconnectFailedException)
            {
                Console.WriteLine("重连服务器失败，请检查网络是否通畅");
                return;
            }

            Console.WriteLine("错误信息" + ea.Exception.ToString());
        }

        private static void OnConnecting(object sender, ClientEvents.ConnectingEventArgs ea)
        {
            var client = sender as AnycastClient;

            if(ea.IsReconnect)
            {
                Console.WriteLine("正常尝试重连服务器:" + ea.ReconnectArgs.Times + "次");
                return;
            }

            Console.WriteLine("正在连接服务器" + client.ServerEndPoint.ToString());
        }

        private static void OnConnected(object sender, ClientEvents.ConnectedEventArgs ea)
        {
            Console.WriteLine("连接服务器成功，按任意键可以退出");
        }
        private static void OnDisconnected(object sender, ClientEvents.DisconnectedEventArgs ea)
        {
            if(ea.IsDropped)
            {
                Console.WriteLine("已掉线");
            }
            Console.WriteLine("已从服务器断开");
        }
    }
}
