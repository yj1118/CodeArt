using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt;
using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Common;
using CodeArt.Concurrent.Sync;

namespace CodeArt.AOP
{
    /// <summary>
    /// 
    /// </summary>
    public static class UnityContext
    {
        #region 边界事件

        public static void DispatchEvent(string eventName, object sender, object arg)
        {
            IList<UnityEvent> events = null;
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

        public static void RegisterEvent(string eventName, UnityEvent handler)
        {
            _eventLock.Write(() =>
            {
                _events.Add(eventName, handler);
            });
        }

        private static MultiDictionary<string, UnityEvent> _events = new MultiDictionary<string, UnityEvent>(false);

        private static MSReaderWriterLock _eventLock = new MSReaderWriterLock();

        #endregion

        #region 边界对象

        public static void Inject<T>(string contextName, ContractImplement implement) where T : class
        {
            Unity.Inject<T>(contextName, implement);
        }

        public static T Resolve<T>(string contextName) where T : class
        {
            return Unity.Resolve<T>(contextName);
        }

        #endregion

    }
}