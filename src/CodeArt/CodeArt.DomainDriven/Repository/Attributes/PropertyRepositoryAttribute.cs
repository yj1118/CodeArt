using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    [DebuggerDisplay("PropertyName = {PropertyName}")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class PropertyRepositoryAttribute : RepositoryAttribute
    {
        private DomainProperty _property;

        /// <summary>
        /// 属性信息
        /// </summary>
        public DomainProperty Property
        {
            get
            {
                return _property;
            }
            internal set
            {
                _property = value;
                InitPropertyType(value);
            }
        }

        /// <summary>
        /// 当属性是集合类型时，获取成员的类型（得到dynamic对应的实际类型）
        /// 如果属性不是集合类型，调用该方法无效
        /// </summary>
        public Type GetElementType()
        {
            if (this.Property.DynamicType != null) return this.Property.DynamicType;
            return this.Property.PropertyType.ResolveElementType();
        }

        public Type PropertyType
        {
            get
            {
                return this.Property.PropertyType;
            }
        }

        public bool IsPublicSet
        {
            get
            {
                return this.Property.AccessLevelSet == PropertyAccessLevel.Public;
            }
        }


        internal DomainPropertyType DomainPropertyType
        {
            get
            {
                return this.Property.DomainPropertyType;
            }
        }

        public string PropertyName
        {
            get
            {
                return this.Property.Name;
            }
        }

        public Type OwnerType
        {
            get
            {
                return this.Property.OwnerType;
            }
        }

        public string Path
        {
            get;
            private set;
        }


        private void InitPropertyType(DomainProperty property)
        {
            this.Path = string.Format("{0}.{1}", this.Property.OwnerType.FullName, this.Property.Name);
            InitEmptyable(property);
        }

        private void InitEmptyable(DomainProperty property)
        {
            var type = typeof(Emptyable<>);
            var propertyType = property.PropertyType;
            var valueType = propertyType.IsList() ? GetElementType() : propertyType;
            if (valueType.IsImplementOrEquals(type))
            {
                var arguments = valueType.GetGenericArguments();
                this.EmptyableConstructor = valueType.ResolveConstructor(arguments);
                this.CreateDefaultEmptyable = this.EmptyableConstructor.DeclaringType.ResolveMethod("CreateEmpty");
                this.EmptyableValueType = (Type)this.EmptyableConstructor.DeclaringType.GetStaticValue("ValueType");
            }
        }


        /// <summary>
        /// 可以使用自定义方法加载该属性，请保证加载属性的方法是数据仓储的静态成员
        /// <para>该方法会在懒惰加载中用到</para>
        /// <para>如果属性在仓储构造函数中出现，那么也会用到该方法</para>
        /// </summary>
        public string LoadMethod
        {
            get;
            set;
        }

        ///// <summary>
        ///// 可以使用自定义方法保存属性，请保证方法是数据仓储的静态成员
        ///// </summary>
        //public string SaveMethod
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 指示属性是否为懒惰加载，即：当用到时才加载
        /// </summary>
        public bool Lazy
        {
            get;
            set;
        }

        /// <summary>
        /// 指示属性是否加入快照
        /// </summary>
        public bool Snapshot
        {
            get;
            set;
        }

        /// <summary>
        /// 指示属性发生变化时，是否记录日志
        /// </summary>
        public bool Log
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为Emptyable类型
        /// </summary>
        public bool IsEmptyable
        {
            get
            {
                return this.EmptyableConstructor != null;
            }
        }

        internal ConstructorInfo EmptyableConstructor
        {
            get;
            private set;
        }

        internal MethodInfo CreateDefaultEmptyable
        {
            get;
            private set;
        }

        internal Type EmptyableValueType
        {
            get;
            private set;
        }


        /// <summary>
        /// 只有标记了该特性的领域属性，ORM才会存到仓储中
        /// </summary>
        public PropertyRepositoryAttribute()
        {
            InitDataAction();
        }

        #region 自定义加载和保存数据

        /// <summary>
        /// 获取运行时类型定义的加载方法，这常用于派生类定义的仓储实现
        /// </summary>
        private Func<Type, MethodInfo> _getLoadData;

        //private Func<Type, MethodInfo> _getSaveData;

        private void InitDataAction()
        {
            _getLoadData = LazyIndexer.Init<Type, MethodInfo>((objectType) =>
            {
                return Repository.GetMethodFromRepository(objectType, this.LoadMethod);
            });

            //_getSaveData = LazyIndexer.Init<Type, MethodInfo>((objectType) =>
            //{
            //    return Repository.GetMethodFromRepository(objectType, this.SaveMethod);
            //});
        }

        /// <summary>
        /// 使用自定义方法加载参数数据
        /// </summary>
        /// <param name="objectType">运行时的实际类型，有可能是派生类的类型</param>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryLoadData(Type objectType, DynamicData data, QueryLevel level, out object value)
        {
            value = null;
            var method = _getLoadData(objectType);
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="objectType"></param>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public virtual bool TrySaveData(Type objectType, object sender, DynamicData data)
        //{
        //    var method = _getSaveData(objectType);
        //    if (method == null) return false;
        //    using (var temp = ArgsPool.Borrow1())
        //    {
        //        var args = temp.Item;
        //        args[0] = sender;
        //        args[1] = data;
        //        method.Invoke(null, args);
        //    }
        //    return true;
        //}

        #endregion

    }
}