using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GodSharp.Opc.Ua;
using GodSharp.Opc.Ua.Client;
using GodSharp.Opc.Ua.Client.Extensions;
using Opc.Ua;
using Opc.Ua.Client;

namespace CodeArt.OPC
{
    public class OPCClient : IDisposable
    {
        private OpcUaClient _client;

        public bool Open()
        {
            return _client.Open();
        }

        public bool Close()
        {
            return _client.Close();
        }

        public bool Subscribe(string type, IEnumerable<string> nodes, Action<string, string, object> callback)
        {
            return _client.Subscribe(type, nodes, (string name, MonitoredItem item, MonitoredItemNotificationEventArgs e) =>
            {
                var nodeId = item.StartNodeId.ToString();
                var lastValue = ((MonitoredItemNotification)item.LastValue).Value.Value;
                callback(name, nodeId, lastValue);
            });
        }

        public bool Unsubscribe(string type)
        {
            return _client.Unsubscribe(type);
        }

        public bool Write((string nodeId, object value)[] items)
        {
            return _client.Write(items);
        }

        /// <summary>
        /// 需要严格注明value类型 例如float类型27f
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Write(string nodeId, object value)
        {
            return _client.Write(nodeId, value);
        }

        public IEnumerable<object> Read(IEnumerable<string> nodeIds)
        {
            var items = _client.Read(nodeIds);
            return items.Select((t) => t.Value);
        }

        public void Dispose()
        {
            this.Close();
        }

        public OPCClient(string url, string username, string password)
        {
            OpcUaClientBuider buider = new OpcUaClientBuider();
            buider.WithEndpoint(url).WithAccount(username, password).WithClientId("OpcUaClient" + Guid.NewGuid().ToString("n"));
            _client = buider.BuildAsync().Result;

            _client.OnSessionConnectNotification = (s, t) =>
            {
                switch (t)
                {
                    case SessionConnectionState.Connected:
                        OPCClientEvents.RaiseSessionConnected(s, new OPCClientEvents.SessionConnectedArgs(s.SessionName));
                        break;
                    case SessionConnectionState.Reconnecting:
                        break;
                    case SessionConnectionState.Disconnecting:
                        break;
                    case SessionConnectionState.Disconnected:
                        OPCClientEvents.RaiseSessionDisconnected(s, new OPCClientEvents.SessionDisconnectedArgs(s.SessionName));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(t), t, null);
                }
            };

            _client.OnSessionKeepAlive = (s, e) =>
            {
                //todo
                //Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}]  {s.SessionName}:{e.CurrentState}");
            };

            _client.OnSessionSubscriptionChanged = subscription =>
            {
                //todo

                //foreach (var item in subscription.Notifications)
                //{
                //    Console.WriteLine("1");
                //}
            };

            _client.OnMonitoredItemNotification = (n, i, e) =>
            {
                //todo

                //foreach (var value in i.DequeueValues())
                //{
                //    Console.WriteLine("{0}->{1} : {2}, {3}, {4}", n, i.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
                //}
            };
        }
    }
}
