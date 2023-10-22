using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CodeArt.EasyMQ.RPC;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Util;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CodeArt.Log;
using CodeArt.EasyMQ;
using CodeArt.AppSetting;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    public class RPCClient : IClient, IDisposable, IMessageHandler
    {
        private string _correlationId;
        private IPoolItem<RabbitBus> _busItem;
        private string _tempQueue;
        private TransferData _result;
        private AutoResetEvent _signal;
        private int _millisecondsTimeout;
        private bool _success;

        public RPCClient(int millisecondsTimeout)
        {
            _busItem = RabbitBus.Borrow(RPC.Policy);
            InitConsumer();
            _signal = new AutoResetEvent(false);
            _millisecondsTimeout = millisecondsTimeout;
        }

        private void InitConsumer()
        {
            var bus = _busItem.Item;
            _tempQueue = bus.TempQueueDeclare();
            bus.Consume(_tempQueue, this);
        }

        public TransferData Invoke(string method, DTObject arg)
        {
            var bus = _busItem.Item;

            _result = default(TransferData);
            _success = false;
            _correlationId = Guid.NewGuid().ToString();

            DTObject dto = DTObject.Create();
            dto["method"] = method;
            dto["arg"] = arg;

            var data = new TransferData(AppSession.Language, dto);
            var routingKey = RPC.GetServerQueue(method.ToLower()); //将服务器端的方法名称作为路由键，统一转为小写表示不区分大小写
            bus.Publish(string.Empty, routingKey, data, (properties) =>
            {
                properties.ReplyTo = _tempQueue;
                properties.CorrelationId = _correlationId;
            });
            _signal.WaitOne(_millisecondsTimeout);

            if (!_success)
            {
                _correlationId = string.Empty;
                throw new RabbitMQException(string.Format(Strings.RequestTimedout, method));
            }

            var info = _result.Info;

            if (info.GetValue<string>("status") == "fail")
            {
                var msg = info.GetValue<string>("message");
                throw new RabbitMQException(string.Format(msg));
            }

            _result.Info = info.GetObject("returnValue");
            return _result;
        }

        public void Handle(Message message)
        {
            if (_correlationId.EqualsIgnoreCase(message.Properties.CorrelationId))
            {
                message.Success();
                _result = message.Content;
                _success = true;
                _signal.Set();
            }
        }

        public void Clear()
        {
            _result = default(TransferData);
            _correlationId = string.Empty;
        }

        public void Dispose()
        {
            _busItem.Dispose();
            _signal.Dispose();
        }




    }
}
