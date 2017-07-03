using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.ModuleNest
{
    public static class ModuleController
    {
        private static Dictionary<string, object> _proxies = new Dictionary<string, object>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Q"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="handlerKey"></param>
        /// <returns></returns>
        public static ModuleHandlerProxy<Q, S> GetHandler<Q, S>(string handlerKey)
            where Q : class
            where S : class
        {
            object proxy = null;
            if (!_proxies.TryGetValue(handlerKey, out proxy))
            {
                lock (_proxies)
                {
                    if (!_proxies.TryGetValue(handlerKey, out proxy))
                    {
                        var handler = ModuleHandlerFactory.GetHandler<Q, S>(handlerKey);
                        proxy = new ModuleHandlerProxy<Q, S>(handlerKey, handler);
                        _proxies.Add(handlerKey, proxy);
                    }
                }
            }
            return proxy as ModuleHandlerProxy<Q, S>;
        }

        public static ModuleHandlerProxy<DTObject, DTObject> GetHandler(string handlerKey)
        {
            return GetHandler<DTObject, DTObject>(handlerKey);
        }
    }
}
