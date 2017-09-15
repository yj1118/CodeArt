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
    /// <summary>
    /// 我们将领域数据分为两部分存放
    /// 一部分是聚合模型内部的成员、另外一部分是引用的聚合根
    /// 由于每个聚合根对象都在领域缓冲区，那么当一个聚合根对象由于失效离开了领域缓冲区，而另外一个在缓冲区内聚合根还持有它的引用
    /// 这时候就会造成不一致性，因此，引用聚合根的数据我们放在appSession里，这样同一线程可以重复使用外部聚合根，而不同的线程
    /// 会在本线程第一次使用外部聚合根时尝试再次加载聚合根，保证一致性
    /// </summary>
    public abstract class DataProxy : IDataProxy
    {
        /// <summary>
        /// 数据代理的唯一键
        /// </summary>
        public string UniqueKey
        {
            get;
            private set;
        }

        private string _appSessionDatasKey;

        public DataProxy()
        {
            this.UniqueKey = Guid.NewGuid().ToString();
            _appSessionDatasKey = string.Format("{0}DataProxyDatas", this.UniqueKey);
        }

        public abstract bool IsSnapshot { get; }

        public abstract bool IsFromSnapshot { get; }

        public abstract int Version { get; set; }

        public abstract void SyncVersion();

        /// <summary>
        /// 共享数据，所有线程对该实例的操作共享此数据
        /// </summary>
        private Dictionary<string, object> _shareDatas = new Dictionary<string, object>();


        private Dictionary<string, object> GetDatasFromAppSession()
        {
            return AppSession.GetOrAddItem(_appSessionDatasKey, () =>
            {
                var pool = DictionaryPool<string, object>.Instance;
                return Symbiosis.TryMark<Dictionary<string, object>>(pool, () =>
                {
                    return new Dictionary<string, object>();
                });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object Load(DomainProperty property)
        {
            return property.IsQuoteAggreateRoot() ? LoadFromAppSession(property) : LoadFromShare(property);
        }

        private object LoadFromShare(DomainProperty property)
        {
            object data = null;
            if (!_shareDatas.TryGetValue(property.Name, out data))
            {
                lock (_shareDatas)  //由于引入了缓冲池机制，我们要保证多线程同时访问一个对象的并发安全
                {
                    if (!_shareDatas.TryGetValue(property.Name, out data))
                    {
                        data = LoadData(property);
                        if (data == null) data = property.GetDefaultValue(this.Owner, property);
                        _shareDatas.Add(property.Name, data);
                    }
                }
            }
            return data;
        }

        private object LoadFromAppSession(DomainProperty property)
        {
            object data = null;
            var datas = GetDatasFromAppSession(); //由于使用了appSession，因此是线程安全的
            if (!datas.TryGetValue(property.Name, out data))
            {
                data = LoadData(property);
                if (data == null) data = property.GetDefaultValue(this.Owner, property);
                datas.Add(property.Name, data);
            }
            return data;
        }



        public virtual bool IsEmpty()
        {
            return false;
        }

        /// <summary>
        /// 外界可以手工指定数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Save(DomainProperty property, object value)
        {
            if (property.IsQuoteAggreateRoot())
                SaveToAppSession(property, value);
            else
                SaveToShare(property, value);
        }

        private void SaveToShare(DomainProperty property, object value)
        {
            //我们对外的承诺是保证Get的线程安全，但是不保证Set的线程安全，所以此处没有做同步处理
            _shareDatas[property.Name] = value;
        }

        private void SaveToAppSession(DomainProperty property, object value)
        {
            var datas = GetDatasFromAppSession();
            datas[property.Name] = value;
        }

        public void Copy(IDataProxy target)
        {
            var t = target as DataProxy;
            if (t == null) return;
            foreach(var p in t._shareDatas)
            {
                if(!_shareDatas.ContainsKey(p.Key))
                {
                    _shareDatas[p.Key] = p.Value;
                }
            }

            var asd = t.GetDatasFromAppSession();
            var localASD = this.GetDatasFromAppSession();
            foreach (var p in asd)
            {
                if (!localASD.ContainsKey(p.Key))
                {
                    localASD[p.Key] = p.Value;
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
            return _shareDatas.ContainsKey(property.Name);
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

            public override int Version
            {
                get
                {
                    return 0;
                }
                set
                {
                    throw new NotImplementedException("SetVersion");
                }
            }

            public override void SyncVersion()
            {
                
            }

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

        public override int Version
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotImplementedException("SetVersion");
            }
        }

        public override void SyncVersion()
        {

        }

    }
}
