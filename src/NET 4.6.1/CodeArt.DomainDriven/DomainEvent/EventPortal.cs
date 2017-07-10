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
    /// 事件门户，可以发布/订阅/取消订阅事件
    /// </summary>
    public static class EventPortal
    {

        /// <summary>
        /// 当事件有订阅时发布事件
        /// </summary>
        public static void Publish<T>(Func<T> getEvent) where T : DomainEvent
        {
            _getEvent(typeof(T)).Publish(getEvent);
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        public static void Subscribe<T>(IDomainEventHandler handler) where T : DomainEvent
        {
            _getEvent(typeof(T)).Subscribe(handler);
        }


        #region 获取事件对象

        private static Func<Type, EventProxy> _getEvent = LazyIndexer.Init<Type, EventProxy>((eventType) =>
        {
            var name = eventType.Name;
            return new EventProxy(name);
        });

        #endregion
    }
}
