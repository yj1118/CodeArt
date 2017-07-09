using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using CodeArt.AppSetting;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.DomainDriven
{
    public static class DomainContext
    {
        /// <summary>
        /// 仅针对类型<typeparamref name="T"/>注册边界事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public static void Register<T>(DomainEvent eventType, DomainEventHandler handler)
            where T : IDomainObject
        {
            Register(typeof(T), eventType, handler);
        }

        /// <summary>
        /// 仅针对类型<paramref name="objectType"/>注册边界事件
        /// </summary>
        /// <typeparam name="objectType"></typeparam>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public static void Register(Type objectType, DomainEvent eventType, DomainEventHandler handler)
        {
            string eventName = GetEventName(objectType, eventType);
            RegisterEvent(eventName, handler);
        }

        /// <summary>
        /// 注册全局领域事件，任何对象的边界操作都会触发该事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public static void Register(DomainEvent eventType, DomainEventHandler handler)
        {
            string eventName = GetEventName(typeof(IDomainObject), eventType);
            RegisterEvent(eventName, handler);
        }

        #region 执行领域事件

        internal static void Execute<T>(DomainEvent eventType, T obj)
            where T : IDomainObject
        {
            Execute(typeof(T), eventType, obj);
        }

        internal static void Execute(Type objectType, DomainEvent eventType, object obj)
        {
            {
                //先触发类型级别的事件
                var args = GetEventArgs(objectType, eventType);
                var eventName = GetEventName(objectType, eventType);
                DispatchEvent(eventName, obj, args);
            }

            {
                //触发全局事件
                var args = GetEventArgs<IDomainObject>(eventType);
                var eventName = GetEventName(typeof(IDomainObject), eventType);
                DispatchEvent(eventName, obj, args);
            }
        }

        #endregion

        #region 获取领域事件的参数数据

        /// <summary>
        /// 获取边界事件上下文对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        /// <returns></returns>
        private static DomainEventArgs GetEventArgs<T>(DomainEvent eventType = DomainEvent.Any)
             where T : IDomainObject
        {
            return GetEventArgs(typeof(T), eventType);
        }

        /// <summary>
        /// 获取事件参数
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        private static DomainEventArgs GetEventArgs(Type objectType, DomainEvent eventType = DomainEvent.Any)
        {
            string eventName = GetEventName(objectType, eventType);
            string argsName = _getBoundedEventArgsName(eventName);
            return AppSession.GetOrAddItem<DomainEventArgs>(argsName, () =>
            {
                return new DomainEventArgs();
            });
        }

        #endregion


        private static string GetEventName(Type objectType, DomainEvent eventType = DomainEvent.Any)
        {
            return _getBoundedEventName(objectType)(eventType);
        }

        private static Func<Type, Func<DomainEvent, string>> _getBoundedEventName =
                    LazyIndexer.Init<Type, Func<DomainEvent, string>>((objectType) =>
                    {
                        return LazyIndexer.Init<DomainEvent, string>((eventType) =>
                        {
                            return string.Format("BoundedEvent_{0}_{1}", objectType.FullName, (byte)eventType);
                        });
                    });


        private static Func<string, string> _getBoundedEventArgsName = LazyIndexer.Init<string, string>((eventName) =>
        {
            return string.Format("{0}_args", eventName);
        });

        #region 数据

        private static void DispatchEvent(string eventName, object sender, DomainEventArgs arg)
        {
            IList<DomainEventHandler> events = null;
            _eventLock.Read(() =>
            {
                _events.TryGetValue(eventName, out events);
            });

            if (events != null)
            {
                foreach (var evt in events)
                {
                    evt(sender, arg);
                }
            }
        }

        private static void RegisterEvent(string eventName, DomainEventHandler handler)
        {
            _eventLock.Write(() =>
            {
                _events.Add(eventName, handler);
            });
        }

        private static MultiDictionary<string, DomainEventHandler> _events = new MultiDictionary<string, DomainEventHandler>(false);

        private static ReaderWriterLockSlim _eventLock = new ReaderWriterLockSlim();

        #endregion
    }
}