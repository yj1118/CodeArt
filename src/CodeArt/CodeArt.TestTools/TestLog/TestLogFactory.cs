using System;
using System.Collections.Generic;

using CodeArt.Concurrent;

namespace CodeArt.TestTools
{
    public static class TestLogFactory
    {
        public static ITestLog Create()
        {
            return Factory.Create();
        }

        private static ITestLogFactory _factory;

        private static ITestLogFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = _factoryByConfig ?? _factoryByRegister;
                    if (_factory == null) throw new TestException("未配置或注入ITestLogFactory的实现");
                }
                return _factory;
            }
        }


        private static ITestLogFactory _factoryByRegister = null;

        /// <summary>
        /// 注册服务提供者工厂，请保证<paramref name="factory"/>是线程安全的
        /// </summary>
        /// <param name="factory"></param>
        public static void Register(ITestLogFactory factory)
        {
            SafeAccessAttribute.CheckUp(factory);
            _factoryByRegister = factory;
        }

        private static ITestLogFactory _factoryByConfig = null;

        static TestLogFactory()
        {
            _factoryByConfig = TestConfiguration.Current.GetLogFactory();
        }

    }
}