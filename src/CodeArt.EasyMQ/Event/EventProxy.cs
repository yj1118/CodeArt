using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.AppSetting;

namespace CodeArt.EasyMQ.Event
{
    internal class EventProxy
    {
        /// <summary>
        /// 事件名称，每一类事件都具有唯一的名称
        /// </summary>
        public string EventName { get; private set; }

        internal EventProxy(string eventName)
        {
            this.EventName = eventName;
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void Publish(IEvent @event)
        {
            var publisher = CreatePublisher();
            publisher.Publish(@event.GetRemotable(), this.EventName);
        }

        private static IPublisher CreatePublisher()
        {
            return EventPortal.GetPublisherFactory().Create();
        }

        /// <summary>
        /// 订阅远程事件
        /// </summary>
        public void Subscribe(IEventHandler handler)
        {
            var subscriber = CreateSubscriber(this.EventName);
            subscriber.AddHandler(handler);
            subscriber.Accept();
        }

        private static ISubscriber CreateSubscriber(string eventName)
        {
            var config = EasyMQConfiguration.Current.EventConfig;
            var group = config.SubscriberGroup;
            return EventPortal.GetSubscriberFactory().Create(eventName, group);
        }

        public void Cancel()
        {
            var subscriber = CreateSubscriber(this.EventName);
            subscriber.Stop();
        }

        public void Cleanup()
        {
            var subscriber = CreateSubscriber(this.EventName);
            subscriber.Cleanup();
        }
    }
}
