using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.Concurrent;

namespace CodeArt.ModuleNest
{
    /// <summary>
    /// 基于<paramref name="ModuleHandlerAttribute"/> 特性的模块处理路由器
    /// </summary>
    [SafeAccess]
    internal sealed class AttributeMHRouter : IModuleHandlerRouter
    {
        private AttributeMHRouter() { }

        public IModuleHandler<Q, S> CreateHandler<Q, S>(string handlerKey)
            where Q : class
            where S : class
        {
            return ModuleHandlerAttribute.GetHandler<Q, S>(handlerKey);
        }

        public static IModuleHandlerRouter Instance = new AttributeMHRouter();

    }
}
