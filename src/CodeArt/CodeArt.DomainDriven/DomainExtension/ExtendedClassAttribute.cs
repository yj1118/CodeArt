using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;

using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 指示该类型用于扩展领域对象
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExtendedClassAttribute : Attribute
    {
        public Type ObjectType
        {
            get;
            private set;
        }

        public Type ExtensionType
        {
            get;
            private set;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType">需要扩展的领域对象类型，也可以是一个扩展类型，当<paramref name="objectOrExtensionType"/>是扩展类型时，这表示三次或三次以上的扩展</param>
        public ExtendedClassAttribute(Type objectOrExtensionType, Type extensionType)
        {
            if (objectOrExtensionType.IsImplementOrEquals(typeof(IDomainObject)))
            {
                this.ObjectType = objectOrExtensionType;
            }
            else
            {
                //objectOrExtensionType是扩展类型，取得特性
                var attr = AttributeUtil.GetAttribute<ExtendedClassAttribute>(objectOrExtensionType);
                if(attr == null) throw new DomainDrivenException(string.Format(Strings.ObjectExtensionAttributeFirstParamError, objectOrExtensionType.FullName));
                this.ObjectType = attr.ObjectType;
            }

            this.ExtensionType = extensionType;
            AddExtensionType(this.ObjectType, extensionType);
        }

        private static void AddExtensionType(Type objectType, Type extensionType)
        {
            lock (_extensionTypes)
            {
                _extensionTypes.TryAdd(objectType, extensionType);
            }
        }

        /// <summary>
        /// 领域对象的类型 -> 为领域对象扩展了职责的类型
        /// </summary>
        private static MultiDictionary<Type, Type> _extensionTypes = new MultiDictionary<Type, Type>(false);

        /// <summary>
        /// 获取领域对象的类型由哪些类型扩展过
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetExtensionTypes(Type objectType)
        {
            return _getExtensionTypes(objectType);
        }

        private static Func<Type, IEnumerable<Type>> _getExtensionTypes = LazyIndexer.Init<Type, IEnumerable<Type>>((objectType) =>
        {
            List<Type> extensionTypes = new List<Type>();
            //顺着继承链查找
            Type currentType = objectType;
            while (currentType != null)
            {
                extensionTypes.AddRange(_extensionTypes.GetValues(currentType));
                currentType = currentType.BaseType;
            }
            return extensionTypes;
        });
        
        internal static void Initialize()
        {
            //主动触发静态构造
            foreach(var info in _extensionTypes)
            {
                var objectType = info.Key;
                //我们在触发扩展类型的静态构造之前，先触发一次原始类型的静态构造，这样避免静态扩展的成员在原始成员之前被注入
                DomainObject.StaticConstructor(objectType);
                var extensionTypes = info.Value;
                foreach(var extensionType in extensionTypes)
                {
                    //触发扩展类型的静态构造函数
                    DomainObject.StaticConstructor(extensionType);
                }
            }
        }

        #region 辅助方法

        /// <summary>
        /// 检查类型是否为领域对象扩展而建立的
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsObjectExtension(Type type)
        {
            return AttributeUtil.IsDefined<ExtendedClassAttribute>(type);
        }

        //public static void CheckObjectExtension(Type type)
        //{
        //    if(!IsObjectExtension(type))
        //}

        /// <summary>
        /// 从扩展类上获取对象的属性方法定义，扩展类中表示属性的获取和设置用的方法是GetXXX,SetXXX，XXX是属性名称，这是我们的约定
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static MethodTuple GetPropertyMethod(Type objectType, string propertyName)
        {
            var extensionTypes = ExtendedClassAttribute.GetExtensionTypes(objectType);
            foreach (var extensionType in extensionTypes)
            {
                //以下代码的意思是，
                var getMethod = extensionType.ResolveMethod(string.Format("Get{0}", propertyName));
                var setMethod = extensionType.ResolveMethod(string.Format("Set{0}", propertyName));
                if (getMethod == null && setMethod == null) continue;
                return new MethodTuple(getMethod, setMethod);
            }
            return MethodTuple.Empty;
        }

        public class MethodTuple
        {
            public MethodInfo Get { get; private set; }
            public MethodInfo Set { get; private set; }

            public MethodTuple(MethodInfo get, MethodInfo set)
            {
                this.Get = get;
                this.Set = set;
            }

            public static readonly MethodTuple Empty = new MethodTuple(null, null);

        }



        #endregion

    }
}
