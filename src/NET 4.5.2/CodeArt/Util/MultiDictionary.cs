using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CodeArt.Util
{
    public sealed class MultiDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
    {
        private Dictionary<TKey, List<TValue>> _data;
        private bool _disposeKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposeKey">当key对应的Value集合的成员数量为0时，是否销毁key,如果销毁key，在下次添加同样的key时，会重新创建list,设置为false，可以节约创建list的开销，但是如果key长期不用，会浪费内存</param>
        public MultiDictionary(bool disposeKey)
        {
            _disposeKey = disposeKey;
            _data = new Dictionary<TKey, List<TValue>>();
        }

        public MultiDictionary(bool disposeKey, IEqualityComparer<TKey> comparer)
        {
            _disposeKey = disposeKey;
            _data = new Dictionary<TKey, List<TValue>>(comparer);
        }

        public MultiDictionary(int capacity, bool disposeKey)
        {
            _data = new Dictionary<TKey, List<TValue>>(capacity);
            _disposeKey = disposeKey;
        }

        public void Add(TKey key, TValue value)
        {
            List<TValue> list = null;
            if (!_data.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                _data.Add(key, list);
            }
            list.Add(value);
        }

        /// <summary>
        /// 如果没有重复项，那么添加value，返回true
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAdd(TKey key, TValue value)
        {
            List<TValue> list = null;
            if (!_data.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                _data.Add(key, list);
            }
            if (list.Contains(value)) return false;
            list.Add(value);
            return true;
        }


        public bool Remove(TKey key)
        {
            return _data.Remove(key);
        }

        public IList<TValue> this[TKey key]
        {
            get
            {
                return _data[key];
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _data.ContainsKey(key);
        }

        public bool RemoveValue(TKey key, TValue value)
        {
            List<TValue> list = null;
            if (_data.TryGetValue(key, out list))
            {
                if (list.Remove(value))
                {
                    if (list.Count == 0 && _disposeKey) _data.Remove(key); //集合为0了，从data中移除
                    return true;
                }
            }
            return false;
        }

        public bool RemoveValue(TKey key, Func<TValue, bool> find)
        {
            List<TValue> list = null;
            if (_data.TryGetValue(key, out list))
            {
                var value = list.FirstOrDefault(find);

                if (value != null && list.Remove(value))
                {
                    if (list.Count == 0 && _disposeKey) _data.Remove(key); //集合为0了，从data中移除
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 删除所有满足条件的value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="find"></param>
        /// <returns></returns>
        public int RemoveValues(TKey key, Predicate<TValue> find)
        {
            List<TValue> list = null;
            int count = 0;
            if (_data.TryGetValue(key, out list))
            {
                count = list.RemoveAll(find);
                if (list.Count == 0 && _disposeKey) _data.Remove(key); //集合为0了，从data中移除
            }
            return count;
        }


        public int RemoveValues(Predicate<TValue> find)
        {
            int count = 0;
            var keys = _data.Keys.ToArray();
            foreach (var key in keys)
            {
                count += RemoveValues(key, find);
            }
            return count;
        }

        public bool TryGetValue(TKey key, out IList<TValue> list)
        {
            List<TValue> temp = null;
            bool result = _data.TryGetValue(key, out temp);
            list = temp;
            return result;
        }

        public TValue GetValue(TKey key, Func<TValue, bool> predicate)
        {
            List<TValue> temp = null;
            if (_data.TryGetValue(key, out temp))
            {
                return temp.FirstOrDefault(predicate);
            }
            return default(TValue);
        }

        public IList<TValue> GetValues(TKey key, Func<TValue, bool> predicate)
        {
            List<TValue> temp = null;
            if (_data.TryGetValue(key, out temp))
            {
                return temp.Where(predicate).ToList();
            }
            return EmptyArray<TValue>.Value;
        }

        public IList<TValue> GetValues(TKey key)
        {
            List<TValue> temp = null;
            if (_data.TryGetValue(key, out temp)) return temp;
            return EmptyArray<TValue>.Value;
        }

        public void Clear()
        {
            _data.Clear();
        }

        public void Each(Action<TValue> action)
        {
            foreach (var p in _data)
            {
                var list = p.Value;
                foreach (var value in list)
                {
                    action(value);
                }
            }
        }

        public IList<TValue> GetValues()
        {
            List<TValue> values = new List<TValue>();
            foreach (var p in _data)
            {
                values.AddRange(p.Value);
            }
            return values;
        }

        public IList<TValue> Where(Func<TValue, bool> predicate)
        {
            List<TValue> result = new List<TValue>();
            foreach (var p in _data)
            {
                var list = p.Value;
                result.AddRange(list.Where(predicate));
            }
            return result;
        }

        public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}