using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 依赖集合
    /// </summary>
    public class DependencyCollection : DependencyObject, IList, ICurable
    {
        public int Add(object value)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.Add(value);
            }
            return this.LocalData.Add(value);
        }

        public void Remove(object obj)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.Remove(obj);
            }
            this.LocalData.Remove(obj);
        }

        public void AddRange(ICollection c)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.AddRange(c);
            }
            this.LocalData.AddRange(c);
        }

        public void Clear()
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.Clear();
            }
            this.LocalData.Clear();
        }

        public void Insert(int index, object value)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.Insert(index, value);
            }
            this.LocalData.Insert(index, value);
        }

        public void InsertRange(int index, ICollection c)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.InsertRange(index, c);
            }
            this.LocalData.InsertRange(index, c);
        }

        public void RemoveAt(int index)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.RemoveAt(index);
            }
            this.LocalData.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.RemoveRange(index, count);
            }
            this.LocalData.RemoveRange(index, count);
        }

        public void Reverse()
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.Reverse();
            }
            this.LocalData.Reverse();
        }

        public void Reverse(int index, int count)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.Reverse(index, count);
            }
            this.LocalData.Reverse(index, count);
        }

        public void SetRange(int index, ICollection c)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.SetRange(index, c);
            }
            this.LocalData.SetRange(index, c);
        }

        public void Sort()
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.Sort();
            }
            this.LocalData.Sort();
        }

        public void Sort(IComparer comparer)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.Sort(comparer);
            }
            this.LocalData.Sort(comparer);
        }

        public void Sort(int index, int count, IComparer comparer)
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.Sort(index, count, comparer);
            }
            this.LocalData.Sort(index, count, comparer);
        }

        public object this[int index]
        {
            get
            {
                return _pinnedData[index];
            }

            set
            {
                if (LoadContext.IsLoading)
                {
                    _pinnedData[index] = value;
                }
                this.LocalData[index] = value;
            }
        }

        public void TrimToSize()
        {
            if (LoadContext.IsLoading)
            {
                _pinnedData.TrimToSize();
            }
            this.LocalData.TrimToSize();
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


        public bool Contains(object value)
        {
            return this.LocalData.Contains(value);
        }
      
        public int IndexOf(object value)
        {
            return this.LocalData.IndexOf(value);
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

        public IEnumerator GetEnumerator()
        {
            return this.LocalData.GetEnumerator();
        }

        #region 固化

        protected override void InitData()
        {
            var localData = this.LocalData;
            localData.Clear();
            foreach (var item in _pinnedData)
            {
                var curable = item as ICurable;
                if (curable != null) curable.LoadPinned();
                localData.Add(item);
            }

            base.InitData();
        }

        public override void OnLoad()
        {
            foreach(var item in this)
            {
                var d = item as DependencyObject;
                if (d != null) d.OnLoad();
            }
            base.OnLoad();
        }

        #endregion

        /// <summary>
        /// 固化数据
        /// </summary>
        private ArrayList _pinnedData = new ArrayList();

        //private ArrayList PinnedData
        //{
        //    get
        //    {
        //        if (_pinnedData == null)
        //        {
        //            var pinned = PinnedDataManager.Current.GetData("collection");
        //            var value = pinned.Load(this.Id) as ArrayList;
        //            if (value == null)
        //            {
        //                value = new ArrayList();
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
        private ArrayList LocalData
        {
            get
            {
                var local = LocalDataManager.Current.GetData(this.ObjectId, "collection");
                var value = local.Load(this.ObjectId) as ArrayList;
                if (value == null)
                {
                    value = new ArrayList();
                    local.Save(this.ObjectId, value);
                }
                return value;
            }
        }

        public DependencyCollection()
        {
        }
    }
}
