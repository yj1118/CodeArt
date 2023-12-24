using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.Web.RPC
{
    /// <summary>
    /// 标记对象是一个过程
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ProcedureAttribute : Attribute
    {
        /// <summary>
        /// 过程的路径
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        public Type LogExtractorType
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">过程名称</param>
        public ProcedureAttribute(string path)
            : this(path, null)
        {
        }

        public ProcedureAttribute(string path, Type logExtractorType)
        {
            this.Path = path.TrimStart("/");  //移除开头的 / ，所有的路径比较都不需要带 "/"，这样比较方便
            this.LogExtractorType = logExtractorType;
        }


        #region 辅助

        private static Func<string, IProcedure> _getProcedure = LazyIndexer.Init<string, IProcedure>((path) =>
        {
            var procedure = GetProcedureImpl(path);
            if(procedure == null)
                throw new ProcedureException(string.Format("没有找到 {0}", path));
            return procedure;
        });

        /// <summary>
        /// 获取服务，该方法不会缓存结果
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static IProcedure GetProcedureImpl(string path)
        {
            RuntimeTypeHandle handle = default(RuntimeTypeHandle);
            path = path.TrimStart("/").ToLower(); //移除开头的 / ，所有的路径比较都不需要带 "/"，这样比较方便
            if (_handlers.TryGetValue(path, out handle))
            {
                var type = Type.GetTypeFromHandle(handle);
                SafeAccessAttribute.CheckUp(type);
                var procedure = Activator.CreateInstance(type) as IProcedure;
                if (procedure == null) throw new TypeMismatchException(type, typeof(IProcedure));
 
                ProcedureAttribute attr = null;
                if(_attributes.TryGetValue(path, out attr))
                {
                    if (attr != null)
                    {
                        procedure.Path = attr.Path;
                        if (attr.LogExtractorType != null)
                        {
                            procedure.LogExtractor = SafeAccessAttribute.CreateSingleton<ILogExtractor>(attr.LogExtractorType);
                        }
                    }
                }
                return procedure;
            }
            return null;
        }


        internal static IProcedure GetProcedure(string procedureName)
        {
            return _getProcedure(procedureName);
        }

        private static IEnumerable<ProcedureAttribute> GetAttributes(Type type)
        {
            return type.GetCustomAttributes(typeof(ProcedureAttribute), true).OfType<ProcedureAttribute>();
        }

        private static Dictionary<string, RuntimeTypeHandle> _handlers = new Dictionary<string, RuntimeTypeHandle>(StringComparer.OrdinalIgnoreCase);

        private static Dictionary<string, ProcedureAttribute> _attributes = new Dictionary<string, ProcedureAttribute>(StringComparer.OrdinalIgnoreCase);

        static ProcedureAttribute()
        {
            //遍历所有类型，找出打上ServiceAttribute标签的类型
            var types = AssemblyUtil.GetImplementTypes(typeof(IProcedure)); //通过接口先找出类型，会大大减少运算量
            foreach (var type in types)
            {
                if (type.IsAbstract || type.IsInterface) continue;
                var attrs = GetAttributes(type);
                foreach(var attr in attrs)
                {
                    var path = attr.Path.ToLower();
                    if (_handlers.ContainsKey(path))
                        throw new ProcedureException(string.Format("{0} 重复", attr.Path));
                    _handlers.Add(path, type.TypeHandle);
                    _attributes.Add(path, attr);
                }
            }
        }

        /// <summary>
        /// 获取当前应用程序由ProcedureAttribute标记的所有过程的名称集合
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetProcedures()
        {
            return _handlers.Keys;
        }

        #endregion
    }
}

