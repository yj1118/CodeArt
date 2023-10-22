using System;
using System.Collections.Generic;

using CodeArt.Concurrent;

namespace CodeArt.ServiceModel
{
    public static class AuthFilterFactory
    {
        public static IAuthFilter Create()
        {
            return _filterByRegister ?? AllowAuthFilter.Instance;
        }


        private static IAuthFilter _filterByRegister = null;

        /// <summary>
        /// 注册授权器，请保证<paramref name="factory"/>是线程安全的
        /// </summary>
        /// <param name="factory"></param>
        public static void Register(IAuthFilter filter)
        {
            SafeAccessAttribute.CheckUp(filter);
            _filterByRegister = filter;
        }

        static AuthFilterFactory()
        {
        }

    }
}