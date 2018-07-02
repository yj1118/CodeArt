using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class EventHandlerAttribute : Attribute
    {
        public Type HandlerType
        {
            get;
            private set;

        }

        public RoutedEvent EventType
        {
            get;
            private set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerType">事件处理器的实现</param>
        /// <param name="eventType"></param>
        public EventHandlerAttribute(Type handlerType, RoutedEvent eventType)
        {
            this.HandlerType = handlerType;
            this.EventType = eventType;
        }

        public EventHandlerAttribute(Type handlerType)
            : this(handlerType, RoutedEvent.Load)
        {
        }

        private static Func<Type, Func<RoutedEvent, IEventHandler[]>> _getHandlers = LazyIndexer.Init<Type, Func<RoutedEvent, IEventHandler[]>>((objectType) =>
        {
            return LazyIndexer.Init<RoutedEvent, IEventHandler[]>((eventType) =>
            {
                var attrs = objectType.GetCustomAttributes<EventHandlerAttribute>(true).ToList();
                List<IEventHandler> handlers = new List<IEventHandler>(attrs.Count);
                foreach (var attr in attrs)
                {
                    if (attr.EventType == eventType)
                    {
                        handlers.Add(attr.CreateHandler());
                    }
                }
                return handlers.ToArray();
            });
        });

        public IEventHandler CreateHandler()
        {
            return SafeAccessAttribute.CreateSingleton<IEventHandler>(this.HandlerType);
        }

        public static IEventHandler[] GetHandlers(Type objectType, RoutedEvent eventType)
        {
            return _getHandlers(objectType)(eventType);
        }

        public static void Process(DependencyObject d,RoutedEvent eventType)
        {
            var handlers = EventHandlerAttribute.GetHandlers(d.ObjectType, eventType);
            foreach (var handler in handlers)
            {
                handler.Process(d, d);
            }
        }

        #region 辅助

        private static EventHandlerAttribute[] AttributeEmpty = Array.Empty<EventHandlerAttribute>();

        #endregion
    }
}
