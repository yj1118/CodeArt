using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.AppSetting;

namespace CodeArt.EasyMQ.Event
{
    /// <summary>
    /// 事件门户，可以发布/订阅/取消订阅事件
    /// </summary>
    public static class EventPortal
    {
        /// <summary>
        /// 当事件有订阅时发布事件
        /// </summary>
        public static void Publish(IEvent @event)
        {
            _getProxy(@event.Name).Publish(@event);
        }


        /// <summary>
        /// 订阅远程事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="branch"></param>
        public static void Subscribe(string eventName, IEventHandler handler)
        {
            _getProxy(eventName).Subscribe(handler);
        }

        public static void Cancel(string eventName)
        {
            _getProxy(eventName).Cancel();
        }

        /// <summary>
        /// 主动释放事件资源
        /// </summary>
        /// <param name="eventName"></param>
        public static void Cleanup(string eventName)
        {
            _getProxy(eventName).Cleanup();
        }


        #region 获取事件对象

        private static Func<string, EventProxy> _getProxy = LazyIndexer.Init<string, EventProxy>((eventName) =>
        {
            return new EventProxy(eventName);
        });

        #endregion

        #region 获取和注册工厂

        internal static IPublisherFactory GetPublisherFactory()
        {
            return _publisherSetting.GetFactory();
        }

        private static FactorySetting<IPublisherFactory> _publisherSetting = new FactorySetting<IPublisherFactory>(() =>
        {
            var config = EasyMQConfiguration.Current.EventConfig;
            InterfaceImplementer imp = config.PublisherFactoryImplementer;
            if (imp != null)
            {
                return imp.GetInstance<IPublisherFactory>();
            }
            return null;
        });

        public static void Register(IPublisherFactory factory)
        {
            _publisherSetting.Register(factory);
        }



        internal static ISubscriberFactory GetSubscriberFactory()
        {
            return _subscriberSetting.GetFactory();
        }

        private static FactorySetting<ISubscriberFactory> _subscriberSetting = new FactorySetting<ISubscriberFactory>(() =>
        {
            var config = EasyMQConfiguration.Current.EventConfig;
            InterfaceImplementer imp = config.SubscriberFactoryImplementer;
            if (imp != null)
            {
                return imp.GetInstance<ISubscriberFactory>();
            }
            return null;
        });

        public static void Register(ISubscriberFactory factory)
        {
            _subscriberSetting.Register(factory);
        }



        #endregion

    }
}
