using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 标记对象是一个服务
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ServiceAttribute : Attribute
    {
        /// <summary>
        /// 服务名称（可以带命名空间）
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">服务名称（可以带命名空间）</param>
        public ServiceAttribute(string name)
        {
            this.Name = name;
        }


        #region 辅助

        private static Func<string, IServiceProvider> _getService = LazyIndexer.Init<string, IServiceProvider>((serviceName) =>
        {
            var service = GetServiceImpl(serviceName);
            if(service == null)
                throw new ServiceException(string.Format(Strings.NoService, serviceName));
            return service;
        });

        /// <summary>
        /// 获取服务，该方法不会缓存结果
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IServiceProvider GetServiceImpl(string serviceName)
        {
            RuntimeTypeHandle serviceHandle = default(RuntimeTypeHandle);
            if (_handlers.TryGetValue(serviceName, out serviceHandle))
            {
                var type = Type.GetTypeFromHandle(serviceHandle);
                SafeAccessAttribute.CheckUp(type);
                var service = Activator.CreateInstance(type) as IServiceProvider;
                if (service == null) throw new TypeMismatchException(type, typeof(IServiceProvider));
                return service;
            }
            return null;
        }


        internal static IServiceProvider GetService(string serviceName)
        {
            return _getService(serviceName);
        }

        private static IEnumerable<ServiceAttribute> GetAttributes(Type type)
        {
            return type.GetCustomAttributes(typeof(ServiceAttribute), true).OfType<ServiceAttribute>();
        }

        private static Dictionary<string, RuntimeTypeHandle> _handlers = new Dictionary<string, RuntimeTypeHandle>(StringComparer.OrdinalIgnoreCase);

        static ServiceAttribute()
        {
            //遍历所有类型，找出打上ServiceAttribute标签的类型
            var types = AssemblyUtil.GetImplementTypes(typeof(IServiceProvider)); //通过接口先找出类型，会大大减少运算量
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface) continue;
                var attrs = GetAttributes(type);
                foreach(var attr in attrs)
                {
                    if (_handlers.ContainsKey(attr.Name))
                        throw new ServiceException(string.Format(Strings.RepeatService, attr.Name));
                    _handlers.Add(attr.Name, type.TypeHandle);
                }
            }
        }

        /// <summary>
        /// 获取当前应用程序由ServiceAttribute标记的所有服务的名称集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetServics()
        {
            return _handlers.Keys;
        }


        #endregion

    }
}

