using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 代码预处理器工厂
    /// </summary>
    public static class CodePreprocessorFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="router"></param>
        public static void Register(ICodePreprocessor instance)
        {
            SafeAccessAttribute.CheckUp(instance.GetType());
            _instance = instance;
        }

        public static ICodePreprocessor Create()
        {
            if (_instance == null) return VirtualPathPreprocessor.Instance;
            return _instance;
        }

        private static ICodePreprocessor _instance = null;
    }
}
