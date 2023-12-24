using System;
using System.Collections.Generic;

using CodeArt.Concurrent;

namespace CodeArt.TestPlatform
{
    public static class VariableProviderFactory
    {
        public static IVariableProvider Create(string package, string name)
        {
            return Factory.Create(package, name);
        }

        private static IVariableProviderFactory _factory;

        private static IVariableProviderFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = _factoryByConfig ?? _factoryByRegister;
                }
                if (_factory == null) throw new TestException("没有配置IVariableProviderFactory的实现，无法使用变量库");
                return _factory;
            }
        }


        private static IVariableProviderFactory _factoryByRegister = null;

        /// <summary>
        /// 注册服务提供者工厂，请保证<paramref name="factory"/>是线程安全的
        /// </summary>
        /// <param name="factory"></param>
        public static void Register(IVariableProviderFactory factory)
        {
            SafeAccessAttribute.CheckUp(factory);
            _factoryByRegister = factory;
        }

        private static IVariableProviderFactory _factoryByConfig = null;

        static VariableProviderFactory()
        {
            _factoryByConfig = TestPlatformConfiguration.Current.Variable.GetProviderFactory();
        }

    }
}