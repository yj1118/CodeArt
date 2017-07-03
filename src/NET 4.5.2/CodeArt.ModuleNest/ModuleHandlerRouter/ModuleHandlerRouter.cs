using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.ModuleNest
{
    internal static class ModuleHandlerRouter
    {
        public static IModuleHandler<Q,S> GetHandler<Q,S>(string handlerKey) where Q : class where S : class
        {
            var route = ModuleHandlerRouterFactory.CreateRoute();
            return route.CreateHandler<Q,S>(handlerKey);
        }

    }
}
