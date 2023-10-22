using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace CodeArt.DTO
{
    internal class DTObjectList : IList<DTObject>
    {
        private List<DTObject> _list = null;

        private DTEList _owner;

        public bool IsPinned
        {
            get;
            private set;
        }

        internal DTObjectList(DTEList owner)
        {
            _owner = owner;
            _list = new List<DTObject>();
        }

        public void Reset()
        {
            this.Clear();
            _list = null;
            _owner = null;
        }

        public DTObject this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                value.Parent = _owner;
                _list[index] = value;
            }
        }

        public IEnumerator<DTObject> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public void Add(DTObject item)
        {
            item.Parent = _owner;
            _list.Add(item);
        }

        public void Clear()
        {
            if(_list != null) //当DTObjectList从池中取出给DTObjects对象使用时，DTObjects和DTObjectList都会被回收，这就会引起DTObjectList二次重置，导致_list为null
                _list.Clear();
        }

        public bool Contains(DTObject item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(DTObject[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(DTObject item)
        {
            if(_list.Remove(item))
            {
                item.Parent = null;
                return true;
            }
            return false;
        }

        public int IndexOf(DTObject item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, DTObject item)
        {
            item.Parent = _owner;
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            if (_list.Count > index)
            {
                _list[index].Parent = null;
                _list.RemoveAt(index);
            }
        }

        public void RemoveAts(IList<int> indexs)
        {
            if (indexs.Count == 0) return;
            var temps = GetItems(indexs);

            foreach (var temp in temps)
            {
                this.Remove(temp);
            }
        }

        public bool Remove(Func<DTObject,bool> predicate)
        {
            if (_list == null || _list.Count == 0) return false;

            var item = _list.FirstOrDefault(predicate);
            if (item != null) return this.Remove(item);
            return false;
        }

        private IList<DTObject> GetItems(IList<int> indexs)
        {
            var items = new List<DTObject>();
            for (var i = 0; i < indexs.Count; i++)
            {
                var pointer = indexs[i];
                if (pointer >= 0 && pointer < _list.Count)
                    items.Add(_list[pointer]);
            }
            return items;
        }

        public void RetainAts(IList<int> indexs)
        {
            var temps = GetItems(indexs);
            var others = _list.Except(temps).ToArray();

            foreach (var other in others)
            {
                this.Remove(other);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #region 自定义方法

        public DTObject[] ToArray()
        {
            return _list.ToArray();
        }

        public T[] ToArray<T>()
        {
            T[] data = new T[_list.Count];
            for (var i = 0; i < _list.Count; i++)
            {
                data[i] = _list[i].GetValue<T>();
            }
            return data;
        }

        public T[] ToArray<T>(Func<DTObject, T> func)
        {
            T[] data = new T[_list.Count];
            for (var i = 0; i < _list.Count; i++)
            {
                data[i] = func(_list[i]);
            }
            return data;
        }

        #endregion


    }
}