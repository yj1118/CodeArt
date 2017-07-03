using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Linq;

using CodeArt.Runtime;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// 应用程序的初始化工作由该类负责，根据不同的上下文环境，在程序入口处调用
    /// AppInitializer.Initialize
    /// 例如：在web环境下，使用PreApplicationStartCode特性
    /// </summary>
    public static class AppInitializer
    {
        private static bool _initialized;
        private static object _syncObject = new object();

        /// <summary>
        /// 应用程序初始化，请根据不同的上下文环境，在程序入口处调用此方法
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;
            lock(_syncObject)
            {
                if (_initialized) return;
                _initialized = true;
                InitPreApplicationStart();
            }
        }

        private static void InitPreApplicationStart()
        {
            var attributes = AssemblyUtil.GetAttributes<PreApplicationStartAttribute>()
                                .OrderByDescending((t) => (byte)t.RunPriority);
            foreach (var attr in attributes)
            {
                attr.Run();
            }
        }
    }
}
