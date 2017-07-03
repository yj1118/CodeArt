using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.ModuleNest
{
    public static class ModuleHandlerFactory
    {
        public static IModuleHandler<Q,S> GetHandler<Q,S>(string handlerKey)
            where Q : class
            where S : class
        {
            return GetByRegistrar<Q, S>(handlerKey) ?? GetByRouter<Q, S>(handlerKey);
        }

        private static IModuleHandler<Q,S> GetByRegistrar<Q, S>(string handlerKey)
            where Q : class
            where S : class
        {
            return ModuleHandlerRegistrar.GetHandler<Q, S>(handlerKey);;
        }

        private static IModuleHandler<Q, S> GetByRouter<Q, S>(string handlerKey)
            where Q : class
            where S : class
        {
            var route = ModuleHandlerRouterFactory.CreateRoute();
            return route.CreateHandler<Q, S>(handlerKey);
        }

        public static void RegisterHandler<Q, S>(string handlerKey, IModuleHandler<Q, S> handler)
            where Q : class
            where S : class
        {
            ModuleHandlerRegistrar.RegisterHandler<Q, S>(handlerKey, handler);
        }

    }
}
