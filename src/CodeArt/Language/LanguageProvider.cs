using CodeArt.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt
{
    public static class LanguageProvider
    {
        /// <summary>
        /// 语言提供其，请保证<paramref name="prodiver"/>是单例的
        /// </summary>
        /// <param name="prodiver"></param>
        public static void Register(ILanguageProvider prodiver)
        {
            SafeAccessAttribute.CheckUp(prodiver.GetType());
            _instance = prodiver;
        }

        public static ILanguageProvider CreateProvider()
        {
            if (_instance == null) return DefaultLanguageProvider.Instance;
            return _instance;
        }

        private static ILanguageProvider _instance = null;
    }
}
