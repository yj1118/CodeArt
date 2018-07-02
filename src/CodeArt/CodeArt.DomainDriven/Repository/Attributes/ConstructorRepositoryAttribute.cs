using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指示属性或对象是需要被仓储的
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = true)]
    public class ConstructorRepositoryAttribute : RepositoryAttribute
    {

        private ConstructorInfo _constructor;

        internal ConstructorInfo Constructor
        {
            get
            {
                return _constructor;
            }
            set
            {
                _constructor = value;
                InitObject(value);
                InitParameters(value);
            }
        }

        public ObjectRepositoryAttribute ObjectTip
        {
            get;
            private set;
        }

        /// <summary>
        /// 构造该对象所用到的仓储接口的类型
        /// </summary>
        public Type RepositoryInterfaceType
        {
            get
            {
                return this.ObjectTip.RepositoryInterfaceType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repositoryType">构造该对象所用到的仓储接口的类型</param>
        public ConstructorRepositoryAttribute()
        {

        }

        private void InitObject(ConstructorInfo constructor)
        {
            var objectType = constructor.DeclaringType;
            this.ObjectTip = ObjectRepositoryAttribute.GetTip(objectType, false); //对于值对象等成员对象可以不标记仓储特性，仓储为聚合根的仓储类型
        }


        #region 参数信息

        private void InitParameters(ConstructorInfo constructor)
        {
            var originals = constructor.GetParameters();
            var prms = new List<ConstructorParameterInfo>(originals.Length);
            foreach (var original in originals)
            {
                var prm = new ConstructorParameterInfo(this, original);
                prms.Add(prm);
            }
            this.Parameters = prms;
        }

        internal IEnumerable<ConstructorParameterInfo> Parameters
        {
            get;
            private set;
        }

        internal sealed class ConstructorParameterInfo
        {
            public ParameterInfo Original
            {
                get;
                private set;
            }

            /// <summary>
            /// 参数的序号
            /// </summary>
            public int Index
            {
                get
                {
                    return this.Original.Position;
                }
            }

            public string Name
            {
                get
                {
                    return this.Original.Name;
                }
            }

            /// <summary>
            /// 获取这个参数对应领域对象的属性的仓储定义，如果不为空，可以自动生成加载代码
            /// </summary>
            public PropertyRepositoryAttribute PropertyTip
            {
                get;
                private set;
            }

            public DomainPropertyType PropertyType
            {
                get
                {
                    return this.PropertyTip.DomainPropertyType;
                }
            }

            public Type DeclaringType
            {
                get
                {
                    return _constructorTip.Constructor.DeclaringType;
                }
            }

            public ParameterRepositoryAttribute Tip
            {
                get;
                private set;
            }

            private ConstructorRepositoryAttribute _constructorTip;
            private MethodInfo _loadData = null;

            /// <summary>
            /// 获取运行时类型定义的加载方法，这常用于派生类定义的仓储实现
            /// </summary>
            private Func<Type, MethodInfo> _getLoadData;


            public ConstructorParameterInfo(ConstructorRepositoryAttribute constructorTip, ParameterInfo original)
            {
                _constructorTip = constructorTip;
                this.Original = original;
                this.Tip = original.GetCustomAttribute<ParameterRepositoryAttribute>(true);
                this.PropertyTip = GetPropertyTip();
                _getLoadData = LazyIndexer.Init<Type, MethodInfo>((objectType) =>
                {
                    if (this.Tip == null || string.IsNullOrEmpty(this.Tip.LoadMethod)) return null;
                    return Repository.GetMethodFromRepository(objectType, this.Tip.LoadMethod);
                });
            }

            private PropertyRepositoryAttribute GetPropertyTip()
            {
                var objectType = _constructorTip.Constructor.ReflectedType;
                var propertyName = this.Original.Name;
                var property = DomainProperty.GetProperty(objectType, propertyName);
                return property.RepositoryTip;
            }


            /// <summary>
            /// 使用自定义方法加载参数数据
            /// </summary>
            /// <param name="data"></param>
            /// <param name="obj"></param>
            /// <returns></returns>
            public bool TryLoadData(Type objectType, DynamicData data, QueryLevel level, out object value)
            {
                value = null;
                var method = _getLoadData(objectType) ?? _loadData;
                if (method == null) return false;
                using (var temp = ArgsPool.Borrow2())
                {
                    var args = temp.Item;
                    args[0] = data;
                    args[1] = level;
                    value = method.Invoke(null, args);
                }
                return true;
            }
        }

        #endregion



        public static ConstructorRepositoryAttribute GetTip(Type objectType)
        {
            return _getTip(objectType);
        }


        private static Func<Type, ConstructorRepositoryAttribute> _getTip = LazyIndexer.Init<Type, ConstructorRepositoryAttribute>((objectType) =>
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var constructors = objectType.GetConstructors(flags);
            var target = constructors.FirstOrDefault((c) =>
            {
                var attr = c.GetCustomAttribute<ConstructorRepositoryAttribute>(true);
                return attr != null;
            });

            if (target == null)
            {
                throw new DomainDrivenException(string.Format(Strings.NoRepositoryConstructor, objectType.FullName));
            }

            var tips = target.GetCustomAttribute<ConstructorRepositoryAttribute>(true);
            tips.Constructor = target;
            return tips;
        });

        /// <summary>
        /// 获得动态类型的构造函数
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static ConstructorInfo GetDynamicConstructor(Type objectType)
        {
            return _getDynamicConstructor(objectType);
        }


        private static Func<Type, ConstructorInfo> _getDynamicConstructor = LazyIndexer.Init<Type, ConstructorInfo>((objectType) =>
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var constructors = objectType.GetConstructors(flags);
            var target = constructors.FirstOrDefault((c) =>
            {
                var attr = c.GetCustomAttribute<ConstructorRepositoryAttribute>(true);
                return attr != null;
            });

            if (target == null)
            {
                throw new DomainDrivenException(string.Format(Strings.NoRepositoryConstructor, objectType.FullName));
            }
            return target;
        });
    }
}
