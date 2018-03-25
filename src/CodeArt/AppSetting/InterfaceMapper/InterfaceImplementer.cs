using CodeArt.Util;
using System;
using System.Collections.Generic;
using System.Xml;

using CodeArt.Concurrent;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// 接口实现器
    /// </summary>
    [SafeAccess]
    public sealed class InterfaceImplementer
    {
        /// <summary>
        /// 接口的实现类型
        /// </summary>
        public Type ImplementType { get; private set; }

        /// <summary>
        /// 创建实现对象时需要传递的构造参数
        /// </summary>
        public object[] Arguments { get; private set; }

        public InterfaceImplementer(Type implementType, object[] arguments)
        {
            this.ImplementType = implementType;
            this.Arguments = arguments;
        }

        public T GetInstance<T>() where T : class
        {
            var instanceType = typeof(T);
            var isSafe = SafeAccessAttribute.IsDefined(instanceType);
            var obj = isSafe ? GetInstanceBySingleton() : CreateInstance();
            var instance = obj as T;
            if (instance == null) throw new TypeMismatchException(obj.GetType(), instanceType);
            return instance;
        }
        
        /// <summary>
        /// 不论对象是否为单例，都创造新的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateInstance<T>() where T : class
        {
            var instanceType = typeof(T);
            var obj = CreateInstance();
            var instance = obj as T;
            if (instance == null) throw new TypeMismatchException(obj.GetType(), instanceType);
            return instance;
        }

        private object _syncObject = new object();
        private object _singletonInstance = null;

        /// <summary>
        /// 以单例的形式得到实例
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private object GetInstanceBySingleton()
        {
            if (_singletonInstance == null)
            {
                lock (_syncObject)
                {
                    if (_singletonInstance == null)
                    {
                        _singletonInstance = CreateInstance();
                    }
                }
            }
            return _singletonInstance;
        }

        private object CreateInstance()
        {
            ArgumentAssert.IsNotNull(this.ImplementType, "ImplementType");
            var instance = Activator.CreateInstance(this.ImplementType, this.Arguments);
            if (instance == null) throw new NoTypeDefinedException(this.ImplementType);
            return instance;
        }


        #region 静态成员

        /// <summary>
        /// 从xml节点中获取定义
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static InterfaceImplementer Create(XmlNode section)
        {
            if (section == null) return null;
            string implementName = section.Attributes["implementType"].Value;
            ArgumentAssert.IsNotNull(implementName, "implementType");

            Type implementType = Type.GetType(implementName);
            if (implementType == null) throw new NoTypeDefinedException(implementName);

            var args = GetArguments(section);

            return new InterfaceImplementer(implementType, args);
        }

        private static object[] GetArguments(XmlNode section)
        {
            var nodes = section.SelectNodes("arguments/add");
            if (nodes.Count == 0) return Array.Empty<object>();

            object[] arguments = new object[nodes.Count];
            for (var i = 0; i < nodes.Count; i++)
            {
                var value = nodes[i].Attributes["value"].Value;
                ArgumentAssert.IsNotNull(value, "value");

                var type = nodes[i].Attributes["type"].Value;
                if (string.IsNullOrEmpty(type)) type = "string";

                object arg = DataUtil.ToValue(value, type);
                if (arg == null) throw new InvalidCastException(string.Format(Strings.OnlySupportedTypes, DataUtil.PrimitiveTypes));
                arguments[i] = arg;
            }
            return arguments;
        }

        #endregion

    }
}