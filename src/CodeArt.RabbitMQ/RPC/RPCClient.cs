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

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 为事件提供广播服务的广播器
    /// </summary>
    public class RPCClient : IClient, IDisposable, IMessageHandler
    {
        private string _correlationId;
        private IPoolItem<RabbitBus> _busItem;
        private string _queue;
        private DTObject _result;
        private AutoResetEvent _signal;
        private int _millisecondsTimeout;

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
            _queue = bus.TempQueueDeclare();
            bus.Consume(_queue, this);
        }

        public DTObject Invoke(string method, DTObject arg)
        {
            var bus = _busItem.Item;

            _result = null;
            _correlationId = Guid.NewGuid().ToString();

            DTObject dto = DTObject.CreateReusable();
            dto["method"] = method;
            dto["args"] = arg;

            var routingKey = RPC.GetServerQueue(method.ToLower()); //将服务器端的方法名称作为路由键，统一转为小写表示不区分大小写
            bus.Publish(string.Empty, routingKey, dto, (properties) =>
            {
                properties.ReplyTo = _queue;
                properties.CorrelationId = _correlationId;
            });
            _signal.WaitOne(_millisecondsTimeout);


            if (_result == null)
            {
                _correlationId = string.Empty;
                throw new RabbitMQException(string.Format(Strings.RequestTimedout, method));
            }

            if (_result.GetValue<string>("status") == "fail")
            {
                var msg = _result.GetValue<string>("message");
                throw new RabbitMQException(string.Format(msg));
            }

            return _result.GetObject("returnValue");
        }

        public void Handle(Message message)
        {
            if (_correlationId.EqualsIgnoreCase(message.Properties.CorrelationId))
            {
                message.Success();
                _result = message.Content;
                _signal.Set();
            }
        }

        public void Clear()
        {
            _result = null;
            _correlationId = string.Empty;
        }

        public void Dispose()
        {
            _busItem.Dispose();
            _signal.Dispose();
        }




    }
}
