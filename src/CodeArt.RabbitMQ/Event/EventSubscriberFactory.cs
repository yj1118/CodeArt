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
    public class EventSubscriberFactory : ISubscriberFactory
    {
        public ISubscriber Create(string eventName, string group)
        {
            return _getSubscriber(eventName)(group);
        }

        private static Func<string, Func<string, EventSubscriber>> _getSubscriber = LazyIndexer.Init<string, Func<string, EventSubscriber>>((eventName) =>
        {
            return LazyIndexer.Init<string, EventSubscriber>((group) =>
            {
                return new EventSubscriber(eventName, group);
            });
        });

        public static readonly EventSubscriberFactory Instance = new EventSubscriberFactory();

    }
}