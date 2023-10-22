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
    /// <summary>
    /// 状态事件，与某一个领域对象的状态相关的事件
    /// </summary>
    public static class StatusEvent
    {
        /// <summary>
        /// 仅针对类型<typeparamref name="T"/>注册状态事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public static void Register<T>(StatusEventType eventType, StatusEventHandler handler)
            where T : IDomainObject
        {
            Register(typeof(T), eventType, handler);
        }

        /// <summary>
        /// 仅针对类型<paramref name="objectType"/>注册状态事件
        /// </summary>
        /// <typeparam name="objectType"></typeparam>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public static void Register(Type objectType, StatusEventType eventType, StatusEventHandler handler)
        {
            Register(objectType.Name, eventType, handler);
        }

        public static void Register(string objectTypeName, StatusEventType eventType, StatusEventHandler handler)
        {
            string eventName = GetEventName(objectTypeName, eventType);
            RegisterEvent(eventName, handler);
        }

        /// <summary>
        /// 注册全局状态事件，任何对象的状态改变都会触发该事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        public static void Register(StatusEventType eventType, StatusEventHandler handler)
        {
            string eventName = GetEventName(typeof(IDomainObject).Name, eventType);
            RegisterEvent(eventName, handler);
        }

        #region 执行状态事件

        internal static void Execute<T>(StatusEventType eventType, T obj)
            where T : IDomainObject
        {
            Execute(typeof(T), eventType, obj);
        }

        internal static void Execute(Type objectType, StatusEventType eventType, object obj)
        {
            {
                //先触发类型级别的事件
                var args = GetEventArgs(objectType);
                var eventName = GetEventName(objectType.Name, eventType);
                DispatchEvent(eventName, obj, args);
            }

            {
                //触发全局事件
                var args = GetEventArgs<IDomainObject>();
                var eventName = GetEventName(typeof(IDomainObject).Name, eventType);
                DispatchEvent(eventName, obj, args);
            }
        }

        #endregion

        #region 获取状态事件的参数数据

        /// <summary>
        /// 获取状态事件的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public static StatusEventArgs GetEventArgs<T>()
             where T : IDomainObject
        {
            return GetEventArgs(typeof(T));
        }

        /// <summary>
        /// 获取事件参数
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="eventType"></param>
        /// <returns></returns>
        private static StatusEventArgs GetEventArgs(Type objectType)
        {
            string eventName = GetEventName(objectType.Name, StatusEventType.Any); //通一会话中，同一个对象类型的状态事件的参数只有一个，不需要通过事件类型再来划分，因为这样用起来不灵活
            string argsName = _getStatusEventArgsName(eventName);
            return AppSession.GetOrAddItem<StatusEventArgs>(argsName, () =>
            {
                return Symbiosis.TryMark<StatusEventArgs>(StatusEventArgs.Pool, () => { return new StatusEventArgs(); });
            });
        }

        #endregion


        private static string GetEventName(string objectTypeName, StatusEventType eventType = StatusEventType.Any)
        {
            return _getStatusEventName(objectTypeName)(eventType);
        }

        private static Func<string, Func<StatusEventType, string>> _getStatusEventName =
                    LazyIndexer.Init<string, Func<StatusEventType, string>>((objectTypeName) =>
                    {
                        return LazyIndexer.Init<StatusEventType, string>((eventType) =>
                        {
                            return string.Format("StatusEvent_{0}_{1}", objectTypeName, (byte)eventType);
                        });
                    });


        private static Func<string, string> _getStatusEventArgsName = LazyIndexer.Init<string, string>((eventName) =>
        {
            return string.Format("{0}_args", eventName);
        });

        #region 数据

        private static void DispatchEvent(string eventName, object sender, StatusEventArgs arg)
        {
            IList<StatusEventHandler> events = null;
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

        private static void RegisterEvent(string eventName, StatusEventHandler handler)
        {
            _eventLock.Write(() =>
            {
                _events.Add(eventName, handler);
            });
        }

        private static MultiDictionary<string, StatusEventHandler> _events = new MultiDictionary<string, StatusEventHandler>(false);

        private static ReaderWriterLockSlim _eventLock = new ReaderWriterLockSlim();

        #endregion
    }
}