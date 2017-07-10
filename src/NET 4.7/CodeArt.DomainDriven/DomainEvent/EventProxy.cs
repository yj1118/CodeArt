using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 领域事件，可以发布/订阅/取消订阅事件
    /// </summary>
    internal class EventProxy
    {
        /// <summary>
        /// 事件名称，每一类领域事件都具有唯一的名称
        /// </summary>
        public string EventName { get; private set; }

        private List<IDomainEventHandler> _handlers;

        internal EventProxy(string eventName)
        {
            this.EventName = eventName;
            _handlers = new List<IDomainEventHandler>();
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void Publish(Func<IDomainEvent> getEvent)
        {
            if (_handlers.Count > 0)
            {
                IDomainEvent @event = getEvent();
                //先发布至本地
                foreach (var handler in _handlers)
                {
                    handler.Handle(@event);
                }
            }
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(IDomainEventHandler handler)
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
    }
}
