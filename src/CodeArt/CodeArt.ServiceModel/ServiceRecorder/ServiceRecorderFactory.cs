using System.ServiceModel;
using CodeArt.ServiceModel;
using System;
using System.Collections.Generic;

using CodeArt.Concurrent;

namespace CodeArt.ServiceModel
{
    public static class ServiceRecorderFactory
    {
        public static IServiceRecorder Create(ServiceRequest request)
        {
            return Factory.Create(request);
        }

        private static IServiceRecorderFactory _factory;

        private static IServiceRecorderFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = _factoryByConfig ?? _factoryByRegister ?? EmptyRecorderFactory.Instance;
                }
                return _factory;
            }
        }


        private static IServiceRecorderFactory _factoryByRegister = null;

        /// <summary>
        /// 注册服务提供者工厂，请保证<paramref name="factory"/>是线程安全的
        /// </summary>
        /// <param name="factory"></param>
        public static void Register(IServiceRecorderFactory factory)
        {
            SafeAccessAttribute.CheckUp(factory);
            _factoryByRegister = factory;
        }

        private static IServiceRecorderFactory _factoryByConfig = null;

        static ServiceRecorderFactory()
        {
            _factoryByConfig = ServiceModelConfiguration.Current.Server.GetRecorderFactory();
        }

    }
}