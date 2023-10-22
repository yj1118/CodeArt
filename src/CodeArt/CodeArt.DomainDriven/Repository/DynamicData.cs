using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public class DynamicData : System.Dynamic.DynamicObject, IDictionary<string, object>
    {
        #region dictionary

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _data.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _data.TryGetValue(key, out value);
        }

        public object this[string name]
        {
            get
            {
                return _data[name];
            }
            set
            {
                _data[name] = value;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return _data.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return _data.Values;
            }
        }

        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        public bool IsEmpty()
        {
            return this.Count == 0;
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _data.Add(item.Key, item.Value);
        }

        public void Combine(DynamicData data)
        {
            foreach (var p in data)
            {
                if (!this.ContainsKey(p.Key))
                    this.Add(p);
            }
        }

        public void Update(DynamicData data)
        {
            foreach (var p in data)
            {
                _data[p.Key] = p.Value;
            }
        }

        public void Clear()
        {
            _data.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_data).Contains(item);
        }


        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)_data).CopyTo(array, arrayIndex);
        }


        public bool Remove(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)_data).Remove(item);
        }


        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return ((ICollection<KeyValuePair<string, object>>)_data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_data).GetEnumerator();
        }

        #endregion

        //保存对象动态定义的属性值  
        private Dictionary<string, object> _data;

        public DynamicData()
        {
            _data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        internal Dictionary<string, object> GetInternalData()
        {
            return _data;
        }

        public object Get(string name)
        {
            if (_data.TryGetValue(name, out var value)) return value;
            return null;
        }

        public void Add(string name, object value)
        {
            _data.Add(name, value);
        }

        /// <summary>
        /// 尝试添加参数，当<paramref name="value"/>为空时候不添加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void TryAdd(string name, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            _data.Add(name, value);
        }

        public void TryAdd(string name, Guid value)
        {
            if (value == Guid.Empty) return;
            _data.Add(name, value);
        }

        public void TryAdd(string name, Guid? value)
        {
            if (!value.HasValue) return;
            _data.Add(name, value.Value);
        }

        public void TryAdd(string name, long? value)
        {
            if (!value.HasValue) return;
            _data.Add(name, value.Value);
        }

        public void TryAdd(string name, int? value)
        {
            if (!value.HasValue) return;
            _data.Add(name, value.Value);
        }

        public void TryAdd(string name, byte? value)
        {
            if (!value.HasValue) return;
            _data.Add(name, value.Value);
        }

        public void TryAdd(string name, bool? value)
        {
            if (!value.HasValue) return;
            _data.Add(name, value.Value);
        }

        public void TryAdd(string name, DateTime? value)
        {
            if (!value.HasValue) return;
            _data.Add(name, value.Value);
        }

        public void TryAdd(string name, IEnumerable value)
        {
            if (value == null) return;
            var hasItem = false;
            foreach(var item in value)
            {
                hasItem = true;
                break;
            }
            if(hasItem)
                _data.Add(name, value);
        }

        public void Set(string name, object value)
        {
            _data[name] = value;
        }

        /// <summary>
        /// 实现动态对象属性成员访问的方法，得到返回指定属性的值
        /// </summary>
        /// <param name="binder"></param>  
        /// <param name="result"></param>  
        /// <returns></returns>  
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.TryGetValue(binder.Name, out result);
            //result = Get(binder.Name);
            //return result == null ? false : true;
        }
        
        /// <summary>  
        /// 实现动态对象属性值设置的方法。  
        /// </summary>  
        /// <param name="binder"></param>  
        /// <param name="value"></param>  
        /// <returns></returns>  
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Add(binder.Name, value);
            return true;
        }

        ///// <summary>  
        ///// 动态对象动态方法调用时执行的实际代码  
        ///// </summary>  
        ///// <param name="binder"></param>  
        ///// <param name="args"></param>  
        ///// <param name="result"></param>  
        ///// <returns></returns>  
        //public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        //{
        //    var theDelegateObj = GetPropertyValue(binder.Name) as DelegateObj;
        //    if (theDelegateObj == null || theDelegateObj.CallMethod == null)
        //    {
        //        result = null;
        //        return false;
        //    }
        //    result = theDelegateObj.CallMethod(this, args);
        //    return true;
        //}

        //public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        //{
        //    return base.TryInvoke(binder, args, out result);
        //}
    }
}
