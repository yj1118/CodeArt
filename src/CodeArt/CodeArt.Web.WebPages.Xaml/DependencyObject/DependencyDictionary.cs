using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 依赖集合
    /// </summary>
    [IgnoreBlank(true)]
    public class DependencyDictionary : DependencyObject, IDictionary, ICurable
    {
        public object this[object key]
        {
            get
            {
                return this.LocalData[key];
            }
            set
            {
                if(LoadContext.IsInitializing)
                {
                    _pinnedData[key] = value;
                }
                this.LocalData[key] = value;
            }
        }


        public bool IsFixedSize
        {
            get
            {
                return this.LocalData.IsFixedSize;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.LocalData.IsReadOnly;
            }
        }

        public ICollection Keys
        {
            get
            {
                return this.LocalData.Keys;
            }
        }

        public ICollection Values
        {
            get
            {
                return this.LocalData.Values;
            }
        }


        public virtual void Add(object key, object value)
        {
            if (LoadContext.IsInitializing)
            {
                _pinnedData.Add(key, value);
            }
            this.LocalData.Add(key, value);
        }

        public void Clear()
        {
            if (LoadContext.IsInitializing)
            {
                _pinnedData.Clear();
            }
            this.LocalData.Clear();
        }

        public bool Contains(object key)
        {
            return this.LocalData.Contains(key);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return this.LocalData.GetEnumerator();
        }

        public void Remove(object key)
        {
            if (LoadContext.IsInitializing)
            {
                _pinnedData.Remove(key);
            }
            this.LocalData.Remove(key);
        }


        public int Count
        {
            get
            {
                return this.LocalData.Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this.LocalData.IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.LocalData.SyncRoot;
            }
        }

        public void CopyTo(Array array, int index)
        {
            this.LocalData.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.LocalData.GetEnumerator();
        }

        #region 固化

        protected override void InitData()
        {
            var localData = this.LocalData;
            localData.Clear();
            foreach (var key in _pinnedData.Keys)
            {
                var value = _pinnedData[key];

                var curable = key as ICurable;
                if (curable != null) curable.LoadPinned();

                curable = value as ICurable;
                if (curable != null) curable.LoadPinned();

                localData.Add(key, value);
            }

            base.InitData();
        }

        #endregion


        /// <summary>
        /// 固化数据
        /// </summary>
        private HybridDictionary _pinnedData = new HybridDictionary();

        //private HybridDictionary PinnedData
        //{
        //    get
        //    {
        //        if (_pinnedData == null)
        //        {
        //            var pinned = PinnedDataManager.Current.GetData("dictionary");
        //            var value = pinned.Load(this.Id) as HybridDictionary;
        //            if (value == null)
        //            {
        //                value = new HybridDictionary();
        //                pinned.Save(this.Id, value);
        //            }
        //            _pinnedData = value;
        //        }
        //        return _pinnedData;
        //    }
        //}


        /// <summary>
        /// 本地数据，仅对当前线程有用
        /// </summary>
        private HybridDictionary LocalData
        {
            get
            {
                var local = LocalDataManager.Current.GetData(this.ObjectId, "dictionary");
                var value = local.Load(this.ObjectId) as HybridDictionary;
                if (value == null)
                {
                    value = new HybridDictionary();
                    local.Save(this.ObjectId, value);
                }
                return value;
            }
        }

        public DependencyDictionary()
        {
        }

      

    }
}
