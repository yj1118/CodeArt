using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace CodeArt.Util
{
    /// <summary>
    /// <para>懒惰索引器</para>
    /// </summary>
    public static class LazyIndexer
    {
        /// <summary>
        /// <para>给予方法懒惰的能力</para>
        /// <para>只有当key第一次出现时才会使用你提供的方法创建value</para>
        /// <para>该机制是线程安全的</para>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static Func<TKey, TValue> Init<TKey, TValue>(Func<TKey, TValue> valueFactory)
        {
            return Init<TKey, TValue>(valueFactory, (value) => { return true; }, EqualityComparer<TKey>.Default);
        }

        public static Func<TKey, TValue> Init<TKey, TValue>(Func<TKey, TValue> valueFactory, IEqualityComparer<TKey> comparer)
        {
            return Init<TKey, TValue>(valueFactory, (value) => { return true; }, comparer);
        }

        public static Func<TKey, TValue> Init<TKey, TValue>(Func<TKey, TValue> valueFactory, Func<TValue, bool> filter)
        {
            return Init<TKey, TValue>(valueFactory, filter, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// <para>创建懒惰索引器</para>
        /// <para>只有当key第一次出现时才会使用你提供的方法创建value</para>
        /// <para>你还可以提供一个IEqualityComparer{TKey}的实现，用于对key进行排序</para>
        /// <para>该机制是线程安全的</para>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="valueFactory">为key创建value的方法</param>
        /// <param name="filter">过滤项，根据value的值确定是否抛弃，返回true表示不抛弃，返回false表示抛弃</param>
        /// <param name="comparer">可以为null,null代表使用默认的排序方式</param>
        /// <returns>确保valueFactory在每个key上只会调用一次,该方法是线程安全的</returns>
        public static Func<TKey, TValue> Init<TKey, TValue>(Func<TKey, TValue> valueFactory, Func<TValue, bool> filter, IEqualityComparer<TKey> comparer)
        {
            if (valueFactory == null) throw new ArgumentNullException("valueFactory");
            var data = new Dictionary<TKey, TValue>(comparer);
            return key =>
            {
                TValue result;
                if (data.TryGetValue(key, out result)) return result;
                lock (data)
                {
                    if (data.TryGetValue(key, out result)) return result;
                    var newValue = valueFactory(key);
                    if (filter == null || filter(newValue))
                    {
                        if (data.TryGetValue(key, out result)) return result;  //为了防止valueFactory内进行了缓存，这里再次判断一下
                        data.Add(key, newValue);
                    }
                    return newValue;
                }
            };
        }
    }

    public class LazyIndexer<TKey, TValue>
    {
        /// <summary>
        /// 使用ConcurrentDictionary，这样读取相关的方法就不用lock(_data)
        /// </summary>
        private ConcurrentDictionary<TKey, TValue> _data = new ConcurrentDictionary<TKey, TValue>();

        private Func<TKey, TValue> _valueFactory;
        private Func<TValue, bool> _filter;
        private IEqualityComparer<TKey> _comparer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="filter">过滤项，根据value的值确定是否抛弃，返回true表示不抛弃，返回false表示抛弃</param>
        public LazyIndexer(Func<TKey, TValue> valueFactory, Func<TValue, bool> filter, IEqualityComparer<TKey> comparer)
        {
            _valueFactory = valueFactory;
            _filter = filter;
            _comparer = comparer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueFactory"></param>
        /// <param name="filter">过滤项，根据value的值确定是否抛弃，返回true表示不抛弃，返回false表示抛弃</param>
        public LazyIndexer(Func<TKey, TValue> valueFactory, Func<TValue, bool> filter)
            : this(valueFactory, filter, EqualityComparer<TKey>.Default)
        {
        }

        public LazyIndexer(Func<TKey, TValue> valueFactory)
            : this(valueFactory, (value) => { return true; }, EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>
        /// 不预设创建工厂，在获取值时候再指定
        /// </summary>
        public LazyIndexer()
            : this(null, (value) => { return true; }, EqualityComparer<TKey>.Default)
        {
        }

        public TValue Get(TKey key)
        {
            if (_valueFactory == null)
                throw new InvalidOperationException("使用LazyIndexer错误，没有预设创建值的工厂，请使用GetOrCreate方法");

            TValue value = default(TValue);

            if (_data.TryGetValue(key, out value)) return value;
            lock (_data)
            {
                if (_data.TryGetValue(key, out value)) return value;
                var newValue = _valueFactory(key);
                if (_filter == null || _filter(newValue)) _data.TryAdd(key, newValue);
                return newValue;
            }
        }

        public TValue Get(TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue value = default(TValue);

            if (_data.TryGetValue(key, out value)) return value;
            lock (_data)
            {
                if (_data.TryGetValue(key, out value)) return value;
                var newValue = valueFactory(key);
                if (_filter == null || _filter(newValue)) _data.TryAdd(key, newValue);
                return newValue;
            }
        }

        /// <summary>
        /// 尝试获取数据，如果没有则返回数据的默认值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public TValue TryGet(TKey key)
        {
            if (_data.TryGetValue(key, out var value)) return value;
            return default(TValue);
        }


        public void Remove(TKey key)
        {
            lock (_data)
            {
                TValue value = default(TValue);
                _data.TryRemove(key, out value);
            }
        }

        public void Clear()
        {
            lock (_data)
            {
                _data.Clear();
            }
        }

        public TKey[] Keys
        {
            get
            {
                return _data.Keys.ToArray();
            }
        }

        public TValue[] Values
        {
            get
            {
                return _data.Values.ToArray();
            }
        }

        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

    }


}
