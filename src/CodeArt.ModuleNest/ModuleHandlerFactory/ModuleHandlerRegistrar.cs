using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.ModuleNest
{
    /// <summary>
    /// 注册仓储员
    /// </summary>
    internal static class ModuleHandlerRegistrar
    {

        #region 获取仓储的实现

        public static IModuleHandler<Q, S> GetHandler<Q, S>(string handlerKey)
            where Q : class
            where S : class
        {
            object handler = null;
            if (_singletons.TryGetValue(handlerKey, out handler)) return (IModuleHandler<Q, S>)handler;
            return null;
        }

        #endregion

        private static Dictionary<string, object> _singletons = new Dictionary<string, object>();

        private static object _syncObject = new object();

        /// <summary>
        /// 注册单例仓储，请确保<paramref name="repository"/>是单例的
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="repository"></param>
        public static void RegisterHandler<Q, S>(string handlerKey, IModuleHandler<Q, S> handler)
            where Q : class
            where S : class
        {
            if (_singletons.ContainsKey(handlerKey)) throw new ModuleException("重复注册模块处理器" + handlerKey);
            lock (_syncObject)
            {
                if (_singletons.ContainsKey(handlerKey)) throw new ModuleException("重复注册模块处理器" + handlerKey);
                SafeAccessAttribute.CheckUp(handler.GetType());
                _singletons.Add(handlerKey, handler);
            }
        }

        public static void RegisterHandler(string handlerKey, IModuleHandler<DTObject, DTObject> handler)
        {
            RegisterHandler<DTObject, DTObject>(handlerKey, handler);
        }
    }
}