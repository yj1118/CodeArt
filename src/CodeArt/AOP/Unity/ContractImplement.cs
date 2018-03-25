using System;
using System.Collections.Generic;

namespace CodeArt.AOP
{
    public sealed class ContractImplement
    {
        /// <summary>
        /// 接口的实现类型
        /// </summary>
        public Type ImplementType { get; set; }

        /// <summary>
        /// 实现类型的实例，是否是单例的
        /// </summary>
        public bool IsSingleton { get; set; }


        public T GetInstance<T>() where T : class
        {
            var obj =  this.IsSingleton ? GetInstanceBySingleton() : CreateInstance();
            var instance = obj as T;
            if (instance == null)
                throw new TypeNotMatchException(obj.GetType(), typeof(T));
            return instance;
        }

        private object _syncObject = new object();
        private object _singleton = null;

        /// <summary>
        /// 以单例的形式得到实例
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private object GetInstanceBySingleton()
        {
            if (_singleton == null)
            {
                lock (_syncObject)
                {
                    if (_singleton == null)
                    {
                        _singleton = CreateInstance();
                    }
                }
            }
            return _singleton;
        }

        private object CreateInstance()
        {
            if (this.ImplementType == null)
                throw new InvalidOperationException("ImplementType为null，无法进行创建实例的操作");
            var instance = Activator.CreateInstance(this.ImplementType);
            if (instance == null) throw new TypeNotFoundException("没有定义类型" + this.ImplementType.FullName);
            return instance;
        }

    }

}