using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Diagnostics;
using CodeArt.Concurrent;

namespace CodeArt.DTO
{
    [DebuggerDisplay("{GetCode(false)}")]
    public class DTObjects : IEnumerable<DTObject>
    {
        private IList<DTObject> _list;

        public DTObjects()
        {
            _list = new List<DTObject>();
        }

        public DTObjects(IList<DTObject> items)
        {
            _list = items;
        }

        public DTObject this[int index]
        {
            get
            {
                return _list[index];
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

        public bool Contains(DTObject item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(DTObject[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int IndexOf(DTObject item)
        {
            return _list.IndexOf(item);
        }

        public void Remove(DTObject item)
        {
            _list.Remove(item);
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 将数组转为一个对象，数组作为对象的成员
        /// </summary>
        /// <param name="memberName">成员名称</param>
        /// <returns></returns>
        public DTObject ToObject(string memberName)
        {
            var obj = DTObject.Create();
            obj.SetList(memberName, _list);
            return obj;
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

        public IEnumerable<T> OfType<T>()
        {
            return ToArray<T>();
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

        /// <summary>
        /// 直接提取枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> ToEnum<T>()
        {
            var enumType = typeof(T);
            return this.Select((t) =>
            {
                var value = t.GetValue<byte>();
                return (T)Enum.ToObject(typeof(T), value);
            });
        }


        public string GetCode(bool sequential)
        {
            using(var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.Append("[");
                foreach (DTObject item in _list)
                {
                    code.Append(item.GetCode(sequential, false));
                    code.Append(",");
                }
                if (_list.Count > 0) code.Length--;
                code.Append("]");
                return code.ToString();
            }
        }

        #endregion

        ///// <summary>
        ///// 集合项是否为单值的
        ///// </summary>
        ///// <returns></returns>
        //public bool ItemIsSingleValue()
        //{
        //    return this.Count > 0 && this[0].IsSingleValue;
        //}

        ///// <summary>
        ///// 尝试将集合转换为单值集合
        ///// </summary>
        ///// <param name="values"></param>
        ///// <returns></returns>
        //public bool TryGetSingleValues(out PrimitiveValueList values)
        //{
        //    values = null;
        //    if (ItemIsSingleValue())
        //    {
        //        values = new PrimitiveValueList(this.Select((v) => v.GetValue()));
        //        return true;
        //    }
        //    return false;
        //}


        public readonly static DTObjects Empty = new DTObjects(new List<DTObject>());

    }
}