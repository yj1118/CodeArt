using System;
using System.Collections.Concurrent;
using System.Threading;

namespace CodeArt.Concurrent
{
    public sealed class PoolWrapper<T> : IDisposable
    {
        private Pool<WrapperItem<T>> _pool;
        private ConcurrentDictionary<T, IPoolItem<WrapperItem<T>>> _map = new ConcurrentDictionary<T, IPoolItem<WrapperItem<T>>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemFactory"></param>
        /// <param name="itemFilter">当项离开或回到池中时，该方法被调用，返回false将告诉池，该项要被抛弃</param>
        /// <param name="config"></param>
        public PoolWrapper(Func<T> itemFactory,
            Func<T, PoolItemPhase, bool> itemFilter,
            PoolConfig config)
        {
            _pool = new Pool<WrapperItem<T>>(() =>
            {
                var t = itemFactory();
                return new WrapperItem<T>(t,_map);
            }, (item, phase) =>
            {
                return itemFilter(item.Value, phase);
            }, config);
        }

        public T Borrow()
        {
            var poolItem = _pool.Borrow();
            _map.TryAdd(poolItem.Item.Value, poolItem); //注意，此处有个前提条件： 针对一个内容，该内容对应的项是唯一的，而不是每次借出的项是不同的，之前框架有此BUG，已经修复
            return poolItem.Item.Value;
        }

        /// <summary>
        /// 设置一个值，表示该项是否是损坏的，如果是损坏的，那么应该从它所属的Pool{T}实例中将该项移除
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isCorrupted"></param>
        public void SetCorrupted(T item, bool isCorrupted)
        {
            IPoolItem<WrapperItem<T>> poolItem = null;
            if (!_map.TryGetValue(item, out poolItem))
                throw new PoolingException(string.Format(Strings.NotFoundPoolItem, item.ToString()));
            poolItem.IsCorrupted = isCorrupted;
        }

        /// <summary>
        /// 获取一个值，表示该项是否是损坏的，如果是损坏的，那么应该从它所属的Pool{T}实例中将该项移除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool GetCorrupted(T item)
        {
            IPoolItem<WrapperItem<T>> poolItem = null;
            if (!_map.TryGetValue(item, out poolItem))
                throw new PoolingException(string.Format(Strings.NotFoundPoolItem, item.ToString()));
            return poolItem.IsCorrupted;
        }

        public void Return(T item)
        {
            if (item == null) return;

            IPoolItem<WrapperItem<T>> poolItem = null;
            if (!_map.TryGetValue(item, out poolItem))
                throw new PoolingException(string.Format(Strings.NotFoundPoolItem, item.ToString()));
            _pool.Return(poolItem);
        }

        public bool IsBorrowedOverstep
        {
            get
            {
                return _pool.IsBorrowedOverstep;
            }
        }

        public int BorrowedCount
        {
            get
            {
                return _pool.BorrowedCount;
            }
        }

        public int WaiterCount
        {
            get
            {
                return _pool.WaiterCount;
            }
        }

        public void Dispose()
        {
            _pool.Dispose();
        }

        private sealed class WrapperItem<V> : IDisposable
        {
            private V _value;
            public V Value
            {
                get { return _value; }
            }

            ConcurrentDictionary<V, IPoolItem<WrapperItem<V>>> _map;

            public WrapperItem(V item, ConcurrentDictionary<V, IPoolItem<WrapperItem<V>>> map)
            {
                _value = item;
                _map = map;
            }

            public void Dispose()
            {
                IPoolItem<WrapperItem<V>> poolItem = null;
                _map.TryRemove(_value, out poolItem);

                var disposable = _value as IDisposable;
                if (disposable != null) 
                    disposable.Dispose();
            }

        }

    }
}
