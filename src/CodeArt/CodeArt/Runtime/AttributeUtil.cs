using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Runtime
{
    public class AttributeUtil
    {
        /// <summary>
        /// 获取类型定义的指定类型的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="defaultAttribute">如果<paramref name="type"/>没有定义特性，那么默认使用<paramref name="defaultAttribute"/>特性</param>
        /// <returns></returns>
        public static T GetAttribute<T>(Type type, T defaultAttribute, bool inherit) where T : Attribute
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), inherit);
            return attributes.Length > 0 ? attributes[0] as T : defaultAttribute;
        }

        /// <summary>
        /// 获取类型定义的指定类型的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="defaultAttribute">如果<paramref name="type"/>没有定义特性，那么默认使用<paramref name="defaultAttribute"/>特性</param>
        /// <returns></returns>
        public static T GetAttribute<T>(Type type, T defaultAttribute) where T : Attribute
        {
            return GetAttribute<T>(type, defaultAttribute, true);
        }

        public static T GetAttribute<T>(Type type, bool inherit) where T : Attribute
        {
            return GetAttribute<T>(type, null, false);
        }

        /// <summary>
        /// 获取类型定义的指定类型的特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            return GetAttribute<T>(type, null);
        }

        public static bool IsDefined<T>(Type type) where T : Attribute
        {
            return type.IsDefined(typeof(T), true);
        }

        public static bool IsDefined<T>(object obj) where T : Attribute
        {
            return IsDefined<T>(obj.GetType());
        }


        /// <summary>
        /// 获取指定对象类型上的属性标记的指定特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(Type objectType, string propertyName) where T : Attribute
        {
            var attributes = GetAttributes(objectType, propertyName);
            return attributes.OfType<T>();
        }

        /// <summary>
        /// 获取指定对象类型上的属性标记的特性
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IEnumerable<Attribute> GetAttributes(Type objectType, string propertyName)
        {
            var propertyInfo = objectType.ResolveProperty(propertyName);
            if (propertyInfo == null) throw new NoPropertyDefinedException(objectType, propertyName);
            return Attribute.GetCustomAttributes(propertyInfo, true);//会在继承链中查找验证规则
        }

        /// <summary>
        /// 获取指定对象类型上标记的特性
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAttributes<T>(Type objectType) where T : Attribute
        {
            return Attribute.GetCustomAttributes(objectType, true).OfType<T>();//会在继承链中查找验证规则
        }


        public static IEnumerable<MethodInfo> GetMethods<T>(Type objectType) where T : Attribute
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;
            var methods = objectType.GetMethods(flags);

            List<MethodInfo> result = new List<MethodInfo>();
            foreach(var method in methods)
            {
                var attr = Attribute.GetCustomAttribute(method, typeof(T));
                if(attr != null)
                {
                    result.Add(method);
                }
            }
            return result;
        }

    }
}
