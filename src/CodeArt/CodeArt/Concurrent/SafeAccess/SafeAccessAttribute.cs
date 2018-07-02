using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// 标示对象并发访问是安全的
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SafeAccessAttribute : Attribute
    {
        public SafeAccessAttribute()
        {
        }

        #region 静态成员

        /// <summary>
        /// 检查类型是否为并发访问安全的
        /// </summary>
        /// <param name="type"></param>
        public static void CheckUp(Type type)
        {
            var access = AttributeUtil.GetAttribute<SafeAccessAttribute>(type, false);
            if (access == null)
                throw new TypeUnsafeAccessException(type);
        }

        public static void CheckUp(object obj)
        {
            CheckUp(obj.GetType());
        }

        public static bool IsDefined(Type type)
        {
            return AttributeUtil.GetAttribute<SafeAccessAttribute>(type) != null;
        }

        public static bool IsDefined(object obj)
        {
            return AttributeUtil.GetAttribute<SafeAccessAttribute>(obj.GetType()) != null;
        }


        private static Func<Type, object> _getSingleInstance = LazyIndexer.Init<Type, object>((objType) =>
        {
            CheckUp(objType);
            return Activator.CreateInstance(objType, true); //设置true，表示就算是私有的构造函数也能匹配
        });

        /// <summary>
        /// 将类型<paramref name="objType"/>以单例的形式创建为<typeparamref name="T"/>的实例
        /// 同一类型不会创建多份实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objType"></param>
        /// <param name="forceSingle"></param>
        /// <returns></returns>
        public static T CreateSingleton<T>(Type objType) where T : class
        {
            T obj = _getSingleInstance(objType) as T;
            if (obj == null) throw new TypeMismatchException(objType, typeof(T));
            return obj;
        }

        public static object CreateSingleton(Type objType)
        {
            return _getSingleInstance(objType);
        }

        /// <summary>
        /// 将类型<typeparamref name="T"/>以单例的形式创建
        /// 同一类型不会创建多份实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objType"></param>
        /// <param name="forceSingle"></param>
        /// <returns></returns>
        public static T CreateSingleton<T>()
        {
            var objType = typeof(T);
            return (T)_getSingleInstance(objType);
        }

        /// <summary>
        /// 将类型<paramref name="objType"/>创建为<typeparamref name="T"/>的实例
        /// <para>如果类型标记了并发访问安全标签，那么会自动以单例的形式创建对象，同一类型不会创建多份实例</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(Type objType) where T : class
        {
            T obj = null;
            var attr = AttributeUtil.GetAttribute<SafeAccessAttribute>(objType);
            if (attr != null)
            {
                obj = _getSingleInstance(objType) as T;
            }
            else
                obj = Activator.CreateInstance(objType, true) as T;//设置true，表示就算是私有的构造函数也能匹配
            if (obj == null) throw new TypeMismatchException(objType, typeof(T));
            return obj;
        }

        public static object CreateInstance(Type objType)
        {
            return CreateInstance<object>(objType);
        }

        /// <summary>
        /// 创建类型<typeparamref name="T"/>的实例
        /// <para>如果类型标记了并发访问安全标签，那么会自动以单例的形式创建对象，同一类型不会创建多份实例</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            var objType = typeof(T);
            object obj = null;
            var attr = AttributeUtil.GetAttribute<SafeAccessAttribute>(objType);
            if (attr != null)
            {
                obj = _getSingleInstance(objType);
            }
            else
                obj = Activator.CreateInstance(objType, true);//设置true，表示就算是私有的构造函数也能匹配
            return (T)obj;
        }


        #endregion
    }
}
