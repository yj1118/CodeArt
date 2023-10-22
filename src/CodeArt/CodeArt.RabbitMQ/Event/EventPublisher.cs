using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.EasyMQ.Event;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.Util;

using RabbitMQ.Client;
using CodeArt.EasyMQ;
using CodeArt.AppSetting;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 为事件提供广播服务的广播器
    /// </summary>
    [SafeAccess]
    public class EventPublisher : IPublisher
    {
        public void Publish(string eventName, DTObject arg)
        {
            using (var temp = RabbitBus.Borrow(Event.Policy))
            {
                var bus = temp.Item;
                bus.ExchangeDeclare(Event.Exchange, ExchangeType.Topic);
                var routingKey = eventName;
                bus.Publish(Event.Exchange, routingKey, new TransferData(AppSession.Language, arg));
            }
        }

        public static readonly EventPublisher Instance = new EventPublisher();
    }
}
