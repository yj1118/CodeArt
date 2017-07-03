using System.ServiceModel;
using CodeArt.ServiceModel;
using System;
using System.Collections.Generic;

using CodeArt.Concurrent;

namespace CodeArt.ServiceModel
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider Create(ServiceRequest request)
        {
            return Factory.Create(request);
        }

        private static IServiceProviderFactory _factory;

        private static IServiceProviderFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = _factoryByConfig ?? _factoryByRegister ?? AttributeProviderFactory.Instance;
                }
                return _factory;
            }
        }


        private static IServiceProviderFactory _factoryByRegister = null;

        /// <summary>
        /// 注册服务提供者工厂，请保证<paramref name="factory"/>是线程安全的
        /// </summary>
        /// <param name="factory"></param>
        public static void Register(IServiceProviderFactory factory)
        {
            SafeAccessAttribute.CheckUp(factory);
            _factoryByRegister = factory;
        }

        private static IServiceProviderFactory _factoryByConfig = null;

        static ServiceProviderFactory()
        {
            _factoryByConfig = ServiceModelConfiguration.Current.Server.GetProviderFactory();
        }

    }
}