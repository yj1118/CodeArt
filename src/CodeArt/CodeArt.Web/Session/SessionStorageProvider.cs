using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Web
{
    internal static class SessionStorageProvider
    {
        #region 获取存储器

        public static ISessionStorage GetStorage()
        {
            ISessionStorage storage = GetStorageByConfig() ?? GetStorageByRegister();
            if (storage == null) throw new WebException("没有定义" + typeof(ISessionStorage).FullName + "的实现");
            return storage;
        }

        private static ISessionStorage GetStorageByConfig()
        {
            var config = WebConfiguration.Current.SessionConfig;
            if (config.SessionStorage == null) return null;
            return config.SessionStorage.GetInstance<ISessionStorage>();
        }

        private static ISessionStorage GetStorageByRegister()
        {
            if (_singletonStorage != null) return _singletonStorage;
            if (_storageType != null) return Activator.CreateInstance(_storageType) as ISessionStorage;
            return null;
        }

        #endregion

        private static object _syncObject = new object();

        private static ISessionStorage _singletonStorage;
        /// <summary>
        /// 注册单例存储器，请确保<paramref name="storage"/>是单例的
        /// </summary>
        /// <param name="storage"></param>
        public static void RegisterSessionStorage(ISessionStorage storage)
        {
            if (NotRegistered())
            {
                lock (_syncObject)
                {
                    if (NotRegistered())
                    {
                        _singletonStorage = storage;
                    }
                }
            }

        }

        private static Type _storageType;

        public static void RegiterSessionStorage(Type storageType, bool isSingleton)
        {
            if (NotRegistered())
            {
                lock (_syncObject)
                {
                    if (NotRegistered())
                    {
                        if (isSingleton)
                        {
                            _singletonStorage = Activator.CreateInstance(storageType) as ISessionStorage;
                        }
                        else
                        {
                            _storageType = storageType;
                        }
                    }
                }
            }
        }


        private static bool NotRegistered()
        {
            return _singletonStorage == null && _storageType == null;
        }

    }
}
