using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

using CodeArt.Log;

namespace CodeArt.Web
{
    public static class Session
    {
        #region 设置项

        /// <summary>
        /// 将数据存入会话状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetItem(string key, object value)
        {
            string sessionId = null;
            if (!TryGetSessionId(ref sessionId)) throw new Exception("无法初始化sessionKey，不能使用session");
            GetStorage().Save(sessionId, key, value);
        }

        private static string LoadSessionKey()
        {
            return GetProvider().LoadKey();
        }

        private static void SaveSessionKey(string value)
        {
            GetProvider().SaveKey(value);
        }

        #endregion

        public static object GetItem(string key)
        {
            if (UnusedSession()) return null;
            string sessionId = null;
            if (TryGetSessionId(ref sessionId)) return GetStorage().Load(sessionId, key);
            return null;
        }

        #region 移除项

        /// <summary>
        /// 移除会话状态中的数据
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveItem(string key)
        {
            if (UnusedSession()) return;
            string sessionId = null;
            if (TryGetSessionId(ref sessionId)) GetStorage().DeleteItem(sessionId, key);
        }

        /// <summary>
        /// 释放当前会话的所有信息
        /// </summary>
        public static void Dispose()
        {
            if (UnusedSession()) return;
            RemoveItems();
            GetProvider().RemoveKey();
        }


        /// <summary>
        /// 删除会话级别的所有数据
        /// </summary>
        public static void RemoveItems()
        {
            if (UnusedSession()) return;
            string sessionId = null;
            if (TryGetSessionId(ref sessionId)) GetStorage().DeleteItems(sessionId);
        }

        /// <summary>
        /// 移除已超过minutes分钟未访问的会话数据
        /// minutes小于或等于0代表删除所有数据
        /// </summary>
        /// <param name="minutes"></param>
        public static void Clear(int minutes)
        {
            GetStorage().Clear(minutes);
        }

        /// <summary>
        /// 没有使用过session存储数据
        /// </summary>
        /// <returns></returns>
        private static bool UnusedSession()
        {
            return !GetProvider().ContainsKey();
        }

        #endregion

        public static string SessionId
        {
            get
            {
                return LoadSessionKey();
            }
            set
            {
                SaveSessionKey(value);
            }
        }

        private static bool TryGetSessionId(ref string sessionId)
        {
            sessionId = LoadSessionKey();
            return sessionId != null;
        }


        #region 存储器

        private static ISessionStorage GetStorage()
        {
            return SessionStorageProvider.GetStorage();
        }

        /// <summary>
        /// 注册单例存储器，请确保<paramref name="storage"/>是单例的
        /// </summary>
        /// <param name="storage"></param>
        public static void RegisterSessionStorage(ISessionStorage storage)
        {
            SessionStorageProvider.RegisterSessionStorage(storage);
        }


        public static void RegiterSessionStorage(Type storageType, bool isSingleton)
        {
            SessionStorageProvider.RegiterSessionStorage(storageType, isSingleton);
        }

        #endregion


        #region sessionKey提供者

        private static ISessionKeyProvider GetProvider()
        {
            return SessionKeyProvider.GetProvider();
        }

        /// <summary>
        /// 注册单例key提供器，请确保<paramref name="provider"/>是单例的
        /// </summary>
        /// <param name="provider"></param>
        public static void RegisterSessionKeyProvider(ISessionKeyProvider provider)
        {
            SessionKeyProvider.RegisterSessionKeyProvider(provider);
        }

        public static void RegisterSessionKeyProvider(Type providerType, bool isSingleton)
        {
            SessionKeyProvider.RegisterSessionKeyProvider(providerType, isSingleton);
        }

        #endregion


        private static Timer _timer;

        static Session()
        {
            InitTimer();
        }


        #region 定时清理

        private static void InitTimer()
        {
            _timer = new Timer(3600 * 1000); //每间隔1小时执行一次
            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
            _timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }

        private static void OnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Clear(24 * 60);
            }
            catch(Exception ex)
            {
                Logger.Fatal(ex);
            }
            finally
            {
                _timer.Start();
            }
        }

        #endregion


    }
}
