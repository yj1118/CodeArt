using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.EasyMQ.Event;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.RabbitMQ
{
    /// <summary>
    /// 为事件提供广播服务的广播器
    /// </summary>
    [SafeAccess]
    public class EventPublisherFactory : IPublisherFactory
    {
        public IPublisher Create()
        {
            return EventPublisher.Instance;
        }


        public static readonly EventPublisherFactory Instance = new EventPublisherFactory();

    }
}