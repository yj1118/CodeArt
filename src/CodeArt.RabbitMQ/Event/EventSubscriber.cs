using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.EasyMQ.Event;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.Log;

using RabbitMQ.Client;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 为事件提供接收服务的接收者
    /// </summary>
    [SafeAccess]
    public class EventSubscriber : ISubscriber, IMessageHandler
    {
        private bool _isWorking;
        private object _syncObject = new object();

        private string _eventName;
        private string _queue;


        public EventSubscriber(string eventName, string group)
        {
            _eventName = eventName;
            _queue = string.Format("{0}-{1}", eventName, group);
            _isWorking = false;
        }

        private bool TryWork()
        {
            if (_isWorking) return false;
            lock (_syncObject)
            {
                if (_isWorking) return false;
                _isWorking = true;
            }
            return true;
        }


        private IPoolItem<RabbitBus> _busItem;

        public void Accept()
        {
            if (!TryWork()) return;

            _busItem = RabbitBus.Borrow(Event.Policy);

            var bus = _busItem.Item;
            bus.ExchangeDeclare(Event.Exchange, ExchangeType.Topic);

            var routingKey = _eventName;
            bus.QueueDeclare(_queue, Event.Exchange, routingKey);
            bus.Consume(_queue, this);
        }


        private bool TryStop()
        {
            if (!_isWorking) return false;
            lock (_syncObject)
            {
                if (!_isWorking) return false;
                _isWorking = false;
            }
            return true;
        }

        public void Stop()
        {
            if (!TryStop()) return;
            _busItem.Dispose();
        }

        public void Cleanup()
        {
            _busItem.Item.QueueDelete(_queue);
        }


        private List<IEventHandler> _handlers = new List<IEventHandler>();

        public void AddHandler(IEventHandler handler)
        {
            lock (_handlers)
            {
                SafeAccessAttribute.CheckUp(handler);
                if (!_handlers.Contains(handler))
                {
                    _handlers.Add(handler);
                }
            }
        }

        /// <summary>
        /// 请自行保证事件的幂等性
        /// </summary>
        /// <param name="message"></param>
        /// <param name="properties"></param>
        /// <param name="ack"></param>
        void IMessageHandler.Handle(Message message)
        {
            //以下代码段反映的是这样一个逻辑：
            //如果是框架产生的异常，那么我们会告诉RabbitMQ服务器重发消息给下一个订阅者
            //如果是事件内部报错，程序员应该自己捕获错误，然后去处理，这时候异常不会被抛出，那么消息就算被处理完了
            //如果事件内部被程序员抛出了异常，那么会被写入日志，并且提示RabbitMQ服务器重发消息给下一个订阅者，重新处理
            //在这种情况下，由于事件会挂载多个，其中一个出错，前面执行的事件也会被重复执行，所以我们要保证事件的幂等性
            var @event = message.Content;
            try
            {
                Parallel.ForEach(_handlers, (handler) =>
                {
                    AppSession.Using(() =>
                    {
                        handler.Handle(@event);
                    }, true);
                });
                message.Success();
            }
            catch (Exception ex)
            {
                LogWrapper.Default.Fatal(ex);
                message.Failed(true); //true:提示RabbitMQ服务器重发消息给下一个订阅者，false:提示RabbitMQ服务器把消息从队列中移除
            }
        }
    }
}