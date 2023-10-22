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
    /// 指示类型为领域派生类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DerivedClassAttribute : Attribute
    {
        public string TypeKey
        {
            get;
            private set;
        }

        public Type DerivedType
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="derivedType">派生类类型</param>
        /// <param name="typeKey">派生类类型对应的唯一标识符，这一般是一个GUID</param>
        public DerivedClassAttribute(Type derivedType, string typeKey)
        {
            this.DerivedType = derivedType;
            this.TypeKey = typeKey;
            lock (_types)
            {
                if(!_types.ContainsKey(typeKey))
                    _types.Add(typeKey, derivedType);
            }
        }

        private static Dictionary<string, Type> _types = new Dictionary<string, Type>();


        public static Type GetDerivedType(string typeKey)
        {
            if (string.IsNullOrEmpty(typeKey)) return null;
            Type type = null;
            if (_types.TryGetValue(typeKey, out type)) return type;
            throw new DomainDrivenException(string.Format(Strings.NotFoundDerivedType, typeKey));
        }

        /// <summary>
        /// 得到<paramref name="baseType"/>的派生类
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetDerivedTypes(Type objectType)
        {
            return _getDerivedTypes(objectType);
        }


        private static Func<Type, IEnumerable<Type>> _getDerivedTypes = LazyIndexer.Init<Type, IEnumerable<Type>>((objectType) =>
        {
            List<Type> types = new List<Type>();
            foreach (var p in _types)
            {
                var type = p.Value;
                if (type.IsImplementOrEquals(objectType)) types.Add(type);
            }
            return types;
        });



        public static DerivedClassAttribute GetAttribute(Type type)
        {
            return type.GetCustomAttribute<DerivedClassAttribute>(false);
        }

        /// <summary>
        /// 检查类型是否标记正确的派生类标签
        /// </summary>
        /// <param name="domainObjectType"></param>
        public static void CheckUp(Type domainObjectType)
        {
            if (IsDerived(domainObjectType))
            {
                var attr = GetAttribute(domainObjectType);
                if (attr == null || string.IsNullOrEmpty(attr.TypeKey))
                    throw new DomainDrivenException(string.Format(Strings.NeedTypeKey, domainObjectType.FullName));
            }
        }

        /// <summary>
        /// 类型<paramref name="domainObjectType"/>是否为另外一个领域类型的派生类
        /// </summary>
        /// <param name="domainObjectType"></param>
        /// <returns></returns>
        public static bool IsDerived(Type domainObjectType)
        {
            if (domainObjectType == null) return false;
            if (!DomainObject.IsDomainObject(domainObjectType)) return false;

            var baseType = domainObjectType.BaseType;
            return baseType != null
                 && !DomainObject.IsMergeDomainType(baseType);
        }


        internal static void Initialize()
        {
            //主动触发静态构造
            foreach (var info in _types)
            {
                var derivedType = info.Value;
                var inheriteds = derivedType.GetInheriteds();
                foreach(var baseType in inheriteds)
                {
                    if (!DomainObject.IsDomainObject(baseType)) continue;
                    //我们在触发扩展类型的静态构造之前，先触发一次原始类型的静态构造，这样避免静态扩展的成员在原始成员之前被注入
                    DomainObject.StaticConstructor(baseType);
                }
                //触发派生类型的静态构造函数
                DomainObject.StaticConstructor(derivedType);
            }
        }

    }
}
