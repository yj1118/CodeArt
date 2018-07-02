using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.ModuleNest
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ModuleHandlerAttribute : Attribute
    {
        public string HandlerKey
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handlerKey"></param>
        public ModuleHandlerAttribute(string handlerKey)
        {
            this.HandlerKey = handlerKey;
        }


        #region 辅助

        private static Func<string, object> _getHandler = LazyIndexer.Init<string, object>((handlerKey) =>
        {
            RuntimeTypeHandle typeHandle = default(RuntimeTypeHandle);
            if (_handlers.TryGetValue(handlerKey, out typeHandle))
            {
                var type = Type.GetTypeFromHandle(typeHandle);
                SafeAccessAttribute.CheckUp(type);
                return Activator.CreateInstance(type);
            }
            throw new ModuleException("没有定义模块处理器" + handlerKey + "的实现");
        });


        internal static IModuleHandler<Q, S> GetHandler<Q, S>(string handlerKey)
            where Q : class
            where S : class
        {
            var instance = _getHandler(handlerKey);
            var handler = instance as IModuleHandler<Q, S>;
            if (handler == null) throw new TypeMismatchException(instance.GetType(), typeof(IModuleHandler<Q, S>));
            return handler;
        }

        private static ModuleHandlerAttribute GetAttribute(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(ModuleHandlerAttribute), true);
            return attributes.Length > 0 ? attributes[0] as ModuleHandlerAttribute : null;
        }

        private static Dictionary<string, RuntimeTypeHandle> _handlers = new Dictionary<string, RuntimeTypeHandle>();

        static ModuleHandlerAttribute()
        {
            //遍历所有类型，找出打上ModuleHandlerAttribute标签的类型
            var types = AssemblyUtil.GetImplementTypes(typeof(IModuleHandler<,>)); //通过接口先找出类型，会大大减少运算量
            foreach(var type in types)
            {
                if (type.IsAbstract || type.IsInterface) continue;
                var attr = GetAttribute(type);
                if (attr != null)
                {
                    if (_handlers.ContainsKey(attr.HandlerKey)) throw new ModuleException("重复定义了模块处理器" + attr.HandlerKey + "的实现");
                    _handlers.Add(attr.HandlerKey, type.TypeHandle);
                }
            }
        }

        #endregion

    }
}
