using System;
using System.Collections.Generic;

using CodeArt.Concurrent;

namespace CodeArt.Web.RPC
{
    public static class ProcedureFactory
    {
        public static IProcedure Create(string virtualPath)
        {
            return Factory.Create(virtualPath);
        }

        private static IProcedureFactory _factory;

        private static IProcedureFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = _factoryByConfig ?? _factoryByRegister ?? AttributeProcedureFactory.Instance;
                }
                return _factory;
            }
        }


        private static IProcedureFactory _factoryByRegister = null;

        /// <summary>
        /// 注册服务提供者工厂，请保证<paramref name="factory"/>是线程安全的
        /// </summary>
        /// <param name="factory"></param>
        public static void Register(IProcedureFactory factory)
        {
            SafeAccessAttribute.CheckUp(factory);
            _factoryByRegister = factory;
        }

        private static IProcedureFactory _factoryByConfig = null;

        static ProcedureFactory()
        {
            //_factoryByConfig = ServiceModelConfiguration.Current.Server.GetProviderFactory();
        }

    }
}