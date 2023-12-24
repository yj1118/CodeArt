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
            return _getSubscriber.Get(eventName).Get(group);
        }

        public IEnumerable<ISubscriber> GetAll()
        {
            lock(_subscribersSyncObject)
            {
                return _subscribers.ToArray();
            }
        }

        private static object _subscribersSyncObject = new object();

        private static List<EventSubscriber> _subscribers = new List<EventSubscriber>();

        private static LazyIndexer<string, LazyIndexer<string, EventSubscriber>> _getSubscriber = new LazyIndexer<string, LazyIndexer<string, EventSubscriber>>((eventName) =>
        {
            return new LazyIndexer<string, EventSubscriber>((group) =>
            {
                var subscriber = new EventSubscriber(eventName, group);
                _subscribers.Add(subscriber);
                return subscriber;
            });
        });


        //private static Func<string, Func<string, EventSubscriber>> _getSubscriber = LazyIndexer.Init<string, Func<string, EventSubscriber>>((eventName) =>
        //{
        //    return LazyIndexer.Init<string, EventSubscriber>((group) =>
        //    {
        //        var subscriber = new EventSubscriber(eventName, group);
        //        _subscribers.Add(subscriber);
        //        return subscriber;
        //    });
        //});


        public ISubscriber Remove(string eventName, string group)
        {
            var @event = _getSubscriber.TryGet(eventName);
            if(@event != null)
            {
                var subscriber = @event.TryGet(group);
                if(subscriber != null)
                {
                    @event.Remove(group);  //移除事件订阅器

                    lock (_subscribersSyncObject)
                    {
                        _subscribers.Remove(subscriber);
                    }
                   
                    if (@event.Count == 0)
                    {
                        _getSubscriber.Remove(eventName);  //将事件集合也删除
                    }
                    return subscriber;
                }
            }
            return null;
        }

        public static readonly EventSubscriberFactory Instance = new EventSubscriberFactory();

    }
}