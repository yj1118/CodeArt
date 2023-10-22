using System;
using System.Collections.Generic;

using CodeArt.Concurrent;

namespace CodeArt.Security
{
    public static class AuthorizerFactory<T> where T : AuthAttribute
    {
        public static IAuthorizer<T> Create()
        {
            return _authorizerByRegister ?? AllowAuthorizer<T>.Instance;
        }

        private static IAuthorizer<T> _authorizerByRegister = null;

        /// <summary>
        /// 注册授权器，请保证<paramref name="factory"/>是线程安全的
        /// </summary>
        /// <param name="factory"></param>
        public static void Register(IAuthorizer<T> authorizer)
        {
            SafeAccessAttribute.CheckUp(authorizer);
            _authorizerByRegister = authorizer;
        }

        //private static IProcedureFactory _factoryByConfig = null;

        static AuthorizerFactory()
        {
            //_factoryByConfig = ServiceModelConfiguration.Current.Server.GetProviderFactory();
        }

    }
}