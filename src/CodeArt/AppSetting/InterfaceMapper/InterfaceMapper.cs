using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;

using CodeArt.Util;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// 接口映射器
    /// </summary>
    public sealed class InterfaceMapper
    {
        private ConcurrentDictionary<Type, InterfaceImplementer> _data = new ConcurrentDictionary<Type, InterfaceImplementer>();

        public InterfaceMapper()
        {

        }

        /// <summary>
        /// 新增一个类型映射
        /// </summary>
        /// <param name="interfaceType">契约</param>
        /// <param name="implementer">实现</param>
        public void AddImplement(Type interfaceType, InterfaceImplementer implementer)
        {
            if(!_data.TryAdd(interfaceType,implementer))
            {
                throw new InvalidOperationException("已经存在类型" + interfaceType.FullName + "的实现");
            }
        }

        public void AddOrUpdateImplement(Type interfaceType, InterfaceImplementer implementer)
        {
            _data.AddOrUpdate(interfaceType, implementer, (t, i) =>
            {
                return implementer;
            });
        }

        public bool Contains(Type interfaceType)
        {
            return _data.ContainsKey(interfaceType);
        }

        public T GetInstance<T>() where T : class
        {
            Type interfaceType = typeof(T);
            InterfaceImplementer implement = null;
            if (!_data.TryGetValue(interfaceType, out implement))
                throw new InvalidOperationException(string.Format(Strings.NoInterfaceImpl, interfaceType.FullName));
            return implement.GetInstance<T>();
        }

        public Type GetInstanceType(Type interfaceType)
        {
            InterfaceImplementer implement = null;
            if (!_data.TryGetValue(interfaceType, out implement))
                throw new InvalidOperationException(string.Format(Strings.NoInterfaceImpl, interfaceType.FullName));
            return implement.ImplementType;
        }


        public InterfaceImplementer[] GetImplementers()
        {
            return _data.Values.ToArray();
        }

        internal InterfaceImplementer GetImplementer(Type interfaceType)
        {
            InterfaceImplementer implement = null;
            if (_data.TryGetValue(interfaceType, out implement)) return implement;
            return null;
        }

        internal InterfaceImplementer GetImplementer<T>()
        {
            return GetImplementer(typeof(T));
        }


        #region 静态成员

        public static InterfaceMapper Create(XmlNode section)
        {
            if (section == null) return null;
            InterfaceMapper mapper = new InterfaceMapper();
            var implements = section.SelectNodes("implement");
            foreach (XmlNode implement in implements)
            {
                string contractTypeName = section.GetAttributeValue("contractType");
                ArgumentAssert.IsNotNull(contractTypeName, "contractTypeName");
                Type contractType = Type.GetType(contractTypeName);
                if (contractType == null)
                    throw new NoTypeDefinedException(contractTypeName);

                var imp = InterfaceImplementer.Create(implement);
                if (imp != null)
                    mapper.AddImplement(contractType, imp);
            }
            return mapper;
        }

        #endregion

    }
}