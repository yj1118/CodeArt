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
    /// 这时候就会造成不一致性，因此，引用聚合根的数据我们放在appSession里，这样同一线程可以重复使用外部聚合根,如果需要知道聚合根是否失效可以通过IsSnapshot判断
    /// 而不同的线程会在本线程第一次使用外部聚合根时尝试再次加载聚合根，保证一致性
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

        private string _appSessionOldDatasKey;

        public DataProxy()
        {
            this.UniqueKey = Guid.NewGuid().ToString(); //这里可以保证就算在同一个线程里，同一个编号但不同实例的领域对象的数据代理是不一样的
            _appSessionDatasKey = string.Format("{0}DataProxyDatas", this.UniqueKey);
            _appSessionOldDatasKey = string.Format("{0}DataProxyOldDatas", this.UniqueKey);
        }

        public abstract bool IsSnapshot { get; }

        public abstract bool IsMirror { get; }

        public abstract bool IsFromSnapshot { get; }

        public abstract int Version { get; set; }

        public abstract void SyncVersion();

        /// <summary>
        /// 因为数据代理可能包含一些线程公共资源，这些资源在对象过期后可以及时清除，腾出内存空间
        /// 数据代理中AppSession就是典型的例子
        /// </summary>
        public void Clear()
        {
            _shareDatas.Clear();
            _shareOldDatas.Clear();
            ClearDatasFromAppSession();
        }

        /// <summary>
        /// 共享数据，所有线程对该实例的操作共享此数据
        /// </summary>
        private Dictionary<string, object> _shareDatas = new Dictionary<string, object>();

        /// <summary>
        /// 用于记录属性更改情况的数据
        /// </summary>
        private Dictionary<string, object> _shareOldDatas = new Dictionary<string, object>();


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

        private Dictionary<string, object> GetOldDatasFromAppSession()
        {
            return AppSession.GetOrAddItem(_appSessionOldDatasKey, () =>
            {
                var pool = DictionaryPool<string, object>.Instance;
                return Symbiosis.TryMark<Dictionary<string, object>>(pool, () =>
                {
                    return new Dictionary<string, object>();
                });
            });
        }

        private void ClearDatasFromAppSession()
        {
            {
                var data = GetDatasFromAppSession();
                data.Clear();
            }

            {
                var data = GetOldDatasFromAppSession();
                data.Clear();
            }
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

        public object LoadOld(DomainProperty property)
        {
            return property.IsQuoteAggreateRoot() ? LoadOldFromAppSession(property) : LoadOldFromShare(property);
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

        private object LoadOldFromShare(DomainProperty property)
        {
            object data = null;
            if (!_shareOldDatas.TryGetValue(property.Name, out data))
            {
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

        private object LoadOldFromAppSession(DomainProperty property)
        {
            object data = null;
            var datas = GetOldDatasFromAppSession(); //由于使用了appSession，因此是线程安全的
            if (!datas.TryGetValue(property.Name, out data))
            {

            }
            return data;
        }



        public virtual bool IsEmpty()
        {
            return false;
        }

        public bool IsNull()
        {
            return this.IsEmpty();
        }

        /// <summary>
        /// 外界可以手工指定数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Save(DomainProperty property, object newValue, object oldValue)
        {
            if (property.IsQuoteAggreateRoot())
                SaveToAppSession(property, newValue, oldValue);
            else
                SaveToShare(property, newValue, oldValue);
        }

        private void SaveToShare(DomainProperty property, object newValue, object oldValue)
        {
            //我们对外的承诺是保证Get的线程安全，但是不保证Set的线程安全，所以此处没有做同步处理
            _shareDatas[property.Name] = newValue;
            if(this.TrackPropertyChange)
                _shareOldDatas[property.Name] = oldValue;
        }

        private void SaveToAppSession(DomainProperty property, object newValue, object oldValue)
        {
            {
                var datas = GetDatasFromAppSession();
                datas[property.Name] = newValue;
            }

            {
                if (this.TrackPropertyChange)
                {
                    var datas = GetOldDatasFromAppSession();
                    datas[property.Name] = oldValue;
                }
            }
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

            if(this.TrackPropertyChange)
            {
                foreach (var p in t._shareOldDatas)
                {
                    if (!_shareOldDatas.ContainsKey(p.Key))
                    {
                        _shareOldDatas[p.Key] = p.Value;
                    }
                }
            }


            //注意，在拷贝数据代理数据时，我们不拷贝外部引用的根信息（这些信息是存在当前线程里的appSession）
            //原因是，不同数据上下文对延迟加载外部根的算法不同，默认的数据上下文是不加载外部根的，只有ORM中的数据上下文才加载
            //var asd = t.GetDatasFromAppSession();
            //var localASD = this.GetDatasFromAppSession();
            //foreach (var p in asd)
            //{
            //    if (!localASD.ContainsKey(p.Key))
            //    {
            //        localASD[p.Key] = p.Value;
            //    }
            //}
        }

        /// <summary>
        /// 拷贝功效数据和当前线程数据，仅用于keep机制
        /// </summary>
        /// <param name="target"></param>
        public void DeepCopy(IDataProxy target)
        {
            var t = target as DataProxy;
            if (t == null) return;
            foreach (var p in t._shareDatas)
            {
                if (!_shareDatas.ContainsKey(p.Key))
                {
                    _shareDatas[p.Key] = p.Value;
                }
            }


            foreach (var p in t._shareOldDatas)
            {
                if (!_shareOldDatas.ContainsKey(p.Key))
                {
                    _shareOldDatas[p.Key] = p.Value;
                }
            }

            {
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

            {
                if (this.TrackPropertyChange)
                {
                    var asd = t.GetOldDatasFromAppSession();
                    var localASD = this.GetOldDatasFromAppSession();
                    foreach (var p in asd)
                    {
                        if (!localASD.ContainsKey(p.Key))
                        {
                            localASD[p.Key] = p.Value;
                        }
                    }
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
            return _shareDatas.ContainsKey(property.Name) || GetDatasFromAppSession().ContainsKey(property.Name);
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

        private bool TrackPropertyChange
        {
            get
            {
                return this.Owner == null ? false : this.Owner.TrackPropertyChange;
            }
        }


        private sealed class DataProxyStorage : DataProxy
        {
            public DataProxyStorage() { }

            protected override object LoadData(DomainProperty property)
            {
                return null;
            }

            public override bool IsSnapshot => false;

            public override bool IsMirror => false;

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

        public override bool IsMirror => false;

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
