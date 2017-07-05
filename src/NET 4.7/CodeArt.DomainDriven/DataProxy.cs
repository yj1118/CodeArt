using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven
{
    public abstract class DataProxy : IDataProxy
    {
        public DataProxy()
        {

        }

        public abstract bool IsSnapshot { get; }

        public abstract bool IsFromSnapshot { get; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object Load(DomainProperty property)
        {
            object data = null;
            if (!_datas.TryGetValue(property.Name, out data))
            {
                lock(_datas)  //由于引入了缓存机制，我们要保证多线程同时访问一个对象的并发安全
                {
                    if (!_datas.TryGetValue(property.Name, out data))
                    {
                        data = LoadData(property);
                        if (data == null) data = property.GetDefaultValue(this.Owner, property);
                        _datas.Add(property.Name, data);
                    }
                }
            }
            return data;
        }


        public virtual bool IsEmpty()
        {
            return false;
        }


        private Dictionary<string, object> _datas = new Dictionary<string, object>();

        /// <summary>
        /// 外界可以手工指定数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Save(DomainProperty property, object value)
        {
            _datas[property.Name] = value;
        }

        public void Copy(IDataProxy target)
        {
            var t = target as DataProxy;
            if (t == null) return;
            foreach(var p in t._datas)
            {
                if(!_datas.ContainsKey(p.Key))
                {
                    _datas[p.Key] = p.Value;
                }
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract object LoadData(DomainProperty property);

        public bool IsLoaded(DomainProperty property)
        {
            return _datas.ContainsKey(property.Name);
        }

        /// <summary>
        /// 创建用于存储的数据代理（不能加载数据，只能存储数据）
        /// </summary>
        /// <returns></returns>
        public static DataProxy CreateStorage(DomainObject owner)
        {
            var proxy = new DataProxyStorage(); 
            proxy.Owner = owner;
            return proxy;
        }

        /// <summary>
        /// 获取该数据代理所属的领域对象实例
        /// </summary>
        public DomainObject Owner
        {
            get;
            set;
        }

        private sealed class DataProxyStorage : DataProxy
        {
            public DataProxyStorage() { }

            protected override object LoadData(DomainProperty property)
            {
                return null;
            }

            public override bool IsSnapshot => false;

            public override bool IsFromSnapshot => false;

        }

        //public virtual void Clear()
        //{
        //    _datas.Clear();
        //    this.Owner = null;
        //}

    }

    public sealed class DataProxyEmpty : DataProxy
    {
        private DataProxyEmpty() { }

        protected override object LoadData(DomainProperty property)
        {
            return null;
        }

        public override bool IsSnapshot => false;

        public override bool IsFromSnapshot => false;

        public static readonly DataProxy Instance = new DataProxyEmpty();
    }
}
