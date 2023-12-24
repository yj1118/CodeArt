using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeArt.Web
{
    internal static class SessionKeyProvider
    {
        #region 获取sessionKey提供者

        public static ISessionKeyProvider GetProvider()
        {
            ISessionKeyProvider provider = GetProviderByConfig() ?? GetProviderByRegister();
            if (provider == null) throw new WebException("没有定义" + typeof(ISessionKeyProvider).FullName + "的实现");
            return provider;
        }

        private static ISessionKeyProvider GetProviderByConfig()
        {
            var config = WebConfiguration.Current.SessionConfig;
            if (config.KeyProvider == null) return null;
            return config.KeyProvider.GetInstance<ISessionKeyProvider>();
        }

        private static ISessionKeyProvider GetProviderByRegister()
        {
            if (_singletonProvider != null) return _singletonProvider;
            if (_providerType != null) return Activator.CreateInstance(_providerType) as ISessionKeyProvider;
            return null;
        }

        #endregion

        private static object _syncObject = new object();

        private static ISessionKeyProvider _singletonProvider;
        /// <summary>
        /// 注册单例key提供器，请确保<paramref name="provider"/>是单例的
        /// </summary>
        /// <param name="provider"></param>
        public static void RegisterSessionKeyProvider(ISessionKeyProvider provider)
        {
            if (NotRegistered())
            {
                lock (_syncObject)
                {
                    if (NotRegistered())
                    {
                        _singletonProvider = provider;
                    }
                }
            }

        }

        private static Type _providerType;

        public static void RegisterSessionKeyProvider(Type providerType, bool isSingleton)
        {
            if (NotRegistered())
            {
                lock (_syncObject)
                {
                    if (NotRegistered())
                    {
                        if (isSingleton)
                        {
                            _singletonProvider = Activator.CreateInstance(providerType) as ISessionKeyProvider;
                        }
                        else
                        {
                            _providerType = providerType;
                        }
                    }
                }
            }
        }


        private static bool NotRegistered()
        {
            return _singletonProvider == null && _providerType == null;
        }

    }
}
