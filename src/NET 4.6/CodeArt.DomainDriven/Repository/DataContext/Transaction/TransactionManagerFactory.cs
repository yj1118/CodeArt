using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DomainDriven
{
    public static class TransactionManagerFactory
    {
        private static ITransactionManagerFactory _factory;

        public static ITransactionManagerFactory CreateFactory()
        {
            if(_factory == null) _factory = _factoryByConfig ?? _factoryByRegister ?? TransactionScopeManagerFactory.Instance;
            return _factory;
        }


        private static ITransactionManagerFactory _factoryByRegister;

        /// <summary>
        /// 以单例的形式，注册事务管理器工厂，请保证<paramref name="factory"/>是单例的
        /// </summary>
        /// <param name="factory"></param>
        public static void RegisterFactory(ITransactionManagerFactory factory)
        {
            _factoryByRegister = factory;
        }

        private static ITransactionManagerFactory _factoryByConfig;

        static TransactionManagerFactory()
        {
            var config = DomainDrivenConfiguration.Current.RepositoryConfig;
            if (config != null) _factoryByConfig = config.GetTransactionManagerFactory();
        }

    }
}