using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace CodeArt.Util
{
    /// <summary>
    /// 根据时间的索引器，该索引器可以按照时间间隔（1,5,10,15,20，30分钟）来缓存数据，当时间失效后，重新加载数据缓存
    /// </summary>
    public class TimeIndexer<TKey, TValue>
    {
        private Func<TKey, TValue> _valueFactory;
        private Func<TValue, bool> _filter;
        private IEqualityComparer<TKey> _comparer;

        private IndexTime _cacheTime;


        /// <summary>
        /// 缓冲时间的间隔大小
        /// </summary>
        private int _cacheIntervalSize;

        /// <summary>
        /// 最小缓存区间（如果20分钟缓存一次，那么缓存区间就是60/20=3 : 0,1,2,最小就是0，最大是2）
        /// </summary>
        private const int _minInterval = 0;

        /// <summary>
        /// 最大缓存区间
        /// </summary>
        private int _maxInterval;

        private int _currentInterval;

        private IndexTimeType _timeType;

        private void Init()
        {
            _cacheIntervalSize = (int)_cacheTime;

            _maxInterval = 60 / _cacheIntervalSize;

            _currentInterval = _minInterval;
        }

        private LazyIndexer<TKey, TValue> _indexer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="filter">过滤项，根据value的值确定是否抛弃，返回true表示不抛弃，返回false表示抛弃</param>
        public TimeIndexer(Func<TKey, TValue> valueFactory, Func<TValue, bool> filter, IEqualityComparer<TKey> comparer, IndexTime cacheTime, IndexTimeType timeType = IndexTimeType.Minute)
        {
            _valueFactory = valueFactory;
            _filter = filter;
            _comparer = comparer;
            _cacheTime = cacheTime;
            _indexer = new LazyIndexer<TKey, TValue>(valueFactory, filter, comparer);
            _timeType = timeType;
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueFactory"></param>
        /// <param name="filter">过滤项，根据value的值确定是否抛弃，返回true表示不抛弃，返回false表示抛弃</param>
        public TimeIndexer(Func<TKey, TValue> valueFactory, Func<TValue, bool> filter, IndexTime cacheTime, IndexTimeType timeType = IndexTimeType.Minute)
            : this(valueFactory, filter, EqualityComparer<TKey>.Default, cacheTime,timeType)
        {
        }

        public TimeIndexer(Func<TKey, TValue> valueFactory, IndexTime cacheTime, IndexTimeType timeType = IndexTimeType.Minute)
            : this(valueFactory, (value) => { return true; }, EqualityComparer<TKey>.Default, cacheTime, timeType)
        {
        }

        public TValue Get(TKey key)
        {
            var timeInterval = GetTimeInterval();
            if (_currentInterval != timeInterval)
            {
                lock (_indexer)
                {
                    if (_currentInterval != timeInterval)
                    {
                        _indexer.Clear();
                        _currentInterval = timeInterval;
                    }
                }
            }

            return _indexer.Get(key);
        }


#if DEBUG

        /// <summary>
        /// 方便单元测试
        /// </summary>
        public DateTime? TestTime
        {
            get;
            set;
        }

#endif

        private int GetTimeInterval()
        {

#if DEBUG
            var current = this.TestTime ?? DateTime.Now;
#endif

#if !DEBUG
            var current = DateTime.Now;
#endif
            var graduate = _timeType == IndexTimeType.Minute ? current.Minute : current.Second;
            var interval = graduate / _cacheIntervalSize;
            if (current.Minute % _cacheIntervalSize > 0) interval++;
            if (interval >= _maxInterval) interval = _minInterval;

            if(_timeType == IndexTimeType.Minute)
            {
                interval += current.Hour * 10;// 比如 11点20分，就是 110+1=111
            }
            else if(_timeType == IndexTimeType.Second)
            {
                interval += current.Hour * 1000 + current.Minute * 10;
            }

            return interval;
        }


        //public TValue GetOrCreate(TKey key, Func<TKey, TValue> valueFactory)
        //{
        //    TValue value = default(TValue);

        //    if (_data.TryGetValue(key, out value)) return value;
        //    lock (_data)
        //    {
        //        if (_data.TryGetValue(key, out value)) return value;
        //        var newValue = valueFactory(key);
        //        if (_filter == null || _filter(newValue)) _data.TryAdd(key, newValue);
        //        return newValue;
        //    }
        //}


        public void Remove(TKey key)
        {
            _indexer.Remove(key);
        }

        public void Clear()
        {
            _indexer.Clear();
        }

    }

    public enum IndexTime : byte
    {
        One = 1,
        Five = 5,
        Ten = 10,
        Fifteen = 15,
        Twenty = 20,
        Thirty = 30
    }

    public enum IndexTimeType : byte
    {
        Minute = 1,
        Second = 2
    }
}
