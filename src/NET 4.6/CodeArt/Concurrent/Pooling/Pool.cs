using System;
using System.Collections.Generic;
using System.Threading;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// 使用池的目的：
    /// 1.避免对象重复创建和销毁，这样会影响性能
    /// 2.限制使用对象的消费者数量，这样可以避免在高并发下，资源被耗尽，服务器当机
    /// 管理共享数量的T的实例
    /// 特点：
    /// 1.使用池版本号，池被清理后，在不影响消费者的使用的前提下，清除已借出的项
    /// 2.借出项的数量限制，不等于池中驻留项的限制
    /// 3.可以选择性的使用析构函数，避免池泄漏，代价是性能下降
    /// </summary>
    /// <typeparam name="T">需要管理的对象类型</typeparam>
	public sealed class Pool<T> : IDisposable
	{
		#region :: 子类 ::

		/// <summary>
        /// 封装由Pool{T}实例拥有的资源
		/// </summary>
		private sealed class ResidentItem
		{
			private bool _isBorrowed;
            private BorrowedItem _borrowedItem;

            /// <summary>
            /// 初始化新的实例
            /// </summary>
            /// <param name="owner">该居住项的拥有者</param>
            /// <param name="item">项</param>
			public ResidentItem(Pool<T> owner, T item)
			{
				Owner = owner;
				Item = item;
                UtcCreated = DateTime.UtcNow;//使用UtcNow使用世界时间，避免时区不同，本地时间不同
				UseCount = 0;
				IsCorrupted = false;
                _borrowedItem = new BorrowedItem(this);
			}

			#region IPoolItem<T> Members

            /// <summary>
            /// 项的拥有者
            /// </summary>
			public Pool<T> Owner { get; private set; }

            /// <summary>
            /// 项
            /// </summary>
			public T Item { get; private set; }

			/// <summary>
            /// 获取或设置一个值，表示该实例是否是损坏的，是否需要从所属的池中被移除
			/// </summary>
			public bool IsCorrupted { get; set; }

			#endregion IPoolItem<T> Members

			/// <summary>
            /// 获取该实例被创建的世界时间
			/// </summary>
			public DateTime UtcCreated { get; private set; }

            /// <summary>
            /// 获取该实例最后一次被使用的时间
            /// </summary>
            public DateTime UtcLastUsed { get; private set; }

			/// <summary>
            /// 获取或者设置该项被使用的次数
			/// </summary>
			public int UseCount { get; private set; }

            /// <summary>
            /// 当项要被借出时，需要调用该方法
            /// 项不能重复借出
            /// </summary>
            /// <param name="poolVersion">项被借出时，池的版本</param>
            /// <returns>封装了借出的项，释放该项时，对象将返回到池中</returns>
			internal IPoolItem<T> Borrow(int poolVersion)
            {
                if (_isBorrowed)
                    throw new InvalidOperationException(string.Format(Strings.RepeatBorrowingPoolItem, typeof(Pool<>)));

                _isBorrowed = true;
                PoolVersionWhenBorrowed = poolVersion;

                return _borrowedItem;
            }

            /// <summary>
            /// 项被借出时，池的版本
            /// </summary>
			internal int PoolVersionWhenBorrowed { get; private set; }

			/// <summary>
            /// 交还对象到所属的池中
            /// 当满足PoolConfig配置的条件时，项可能会被丢弃，并且相关的资源会被释放
			/// </summary>
			public void Return()
			{
				if (!_isBorrowed) throw new InvalidOperationException(Strings.CannotReturnPoolItem);
				_isBorrowed = false;
                this.UtcLastUsed = DateTime.UtcNow; //更新最后一次使用的时间
                ++UseCount;
				Owner._return(this);
			}
		}

        /// <summary>
        /// 封装被借出的项
        /// </summary>
		private class BorrowedItem : IPoolItem<T>
		{
			private readonly ResidentItem _parent;

			public BorrowedItem(ResidentItem parent)
			{
				_parent = parent;
			}

			public Pool<T> Owner
			{
				get { return _parent.Owner; }
			}

			public T Item
			{
				get { return _parent.Item; }
			}

            /// <summary>
            /// 或者或设置一个值，表示该项是否已损坏
            /// </summary>
			public bool IsCorrupted
			{
				get
				{
					return _parent.IsCorrupted;
				}
				set
				{
					_parent.IsCorrupted = value;
				}
			}

			public void Dispose()
			{
                _parent.Return();
			}
		}

        #endregion :: 子类 ::

        /// <summary>
        /// 同步对象
        /// </summary>
        private readonly object _syncRoot = new object();
		private readonly Func<T> _itemFactory;
        private readonly Func<T, PoolItemPhase, bool> _itemFilter;
        private readonly Action<T> _itemDestroyer; //毁灭者，当项被消除时，会使用该对象进行额外的销毁操作
        private IPoolContainer<ResidentItem> _container;
		private bool _isDisposed;
		private int _poolVersion;

		private readonly PoolFetchOrder _fetchOrder;
        /// <summary>
        /// 设置或获取在同一个时间内，最大能够借出的项的数量。
        /// 如果线程池中借出的项数量达到该值，那么下次在借用项时，调用线程将被阻塞，直到有项被返回到线程池中。
        /// 如果该值小于或者等于0，那么项会被马上借给调用线程，默认值是0（无限）	
        /// </summary>
		private readonly int _loanCapacity;
        /// <summary>
        /// 获取或设置池中可容纳的最大项数量
        /// 当项被返回到池中时，如果池的容量已达到最大值，那么该项将被抛弃。
        /// 如果该值小于或等于0，代表无限制
        /// </summary>
		private readonly int _poolCapacity;

        /// <summary>
        /// 获取或设置池中每一项的最大寿命（单位秒）
        /// 如果该值小于或者等于0，则代表允许池中的项无限制存在
        /// </summary>
		private readonly int _maxLifespan;

        /// <summary>
        /// 停留时间（单位秒）
        /// 如果项在池中超过停留时间，那么抛弃
        /// 如果项被使用，那么停留时间会被重置计算
        /// 如果该值小于或者等于0，则代表允许池中的项无限制存在
        /// </summary>
        private readonly int _maxRemainTime;

        /// <summary>
        /// 获取或设置池中项在被移除或销毁之前，能使用的最大次数
        /// 如果该值小于或等于0，那么可以使用无限次
        /// </summary>
		private readonly int _maxUses;

        private int _borrowedCount;
        /// <summary>
        /// 已借出项的个数
        /// </summary>
        public int BorrowedCount
        {
            get
			{
				lock (_syncRoot)
				{
					if (_isDisposed) throw new ObjectDisposedException(this.GetType().Name);
					return _borrowedCount;
				}
			}
        }

        private int _waiterCount;
        /// <summary>
        /// 等待的消费者个数
        /// </summary>
        public int WaiterCount
        {
            get
			{
				lock (_syncRoot)
				{
					if (_isDisposed) throw new ObjectDisposedException(this.GetType().Name);
					return _waiterCount;
				}
			}
        }


        /// <summary>
        /// 	<para>初始化一个新的池实例</para>
        /// </summary>
        /// <param name="itemFactory">
        ///	<para>当池中没有可以重用的项时,用其创建一个项.</para>
        /// </param>
        /// <param name="itemFilter">
        ///	<para>
        ///	当项离开或者回到池中时，会调用这个方法。
        ///	如果项可以继续重用，返回true
        ///	如果项需要被抛弃并从池中被移除，那么返回false
        ///	当项是返回池中时， phase 是 PoolItemPhase.Returning
        ///	当项是离开池中时，phase 是PoolItemPhase.Leaving,离开是指从池中移除，而不是借出去
        ///	</para>
        /// </param>
        /// <param name="config">
        ///	<para>池行为的配置</para>
        /// </param>
        /// <example>
        ///	<code>
        ///		public class MyClass
        ///		{
        ///			PoolConfig _config;
        ///			Pool&lt;MyConnection&gt; _pool;
        ///				
        ///			public MyClass()
        ///			{
        ///				_config = new PoolConfig()
        ///				{
        ///					FetchOrder = PoolFetchOrder.Fifo; // 先进先出
        ///					LoanCapacity = 10; // 同一时间内，不会有超过10个连接被借出
        ///					PoolCapacity = 0; // 池内会有无限个连接可以被使用 （不过实际不会有无限个，因为在同一时间不会有超过10个连接会被借出)
        ///					MaxUses = 0; // 无限
        ///					MaxLifespan = 60 * 5 // 5分钟后过期
        ///				};
        ///				_pool = new Pool&lt;MyConnection&gt;(
        ///				
        ///				//当池中没有有效的连接被使用时，该方法会被调用
        ///				() =>
        ///				{
        ///					var conn = new MyConnection();
        ///					conn.Open();
        ///					return conn;
        ///				},
        ///				
        ///				//当项离开或回到池中时，该方法被调用
        ///				(connection, phase) =>
        ///				{
        ///					//返回false将告诉池，该项要被抛弃
        ///					if(!connection.IsOpen) return false;
        ///					/连接是返回池中时，清理连接
        ///					if(phase == PoolItemPhase.Returning) connection.ClearBuffer();
        ///					return true;
        ///				},
        ///				
        ///				_config);
        ///			}
        ///		
        ///			public void SendSomeData(byte[] data)
        ///			{
        ///				using(var conn = _pool.Borrow())
        ///				{
        ///					try
        ///					{
        ///						conn.Item.SendSomeDataTo("127.0.0.1", data);
        ///					}
        ///					catch(MyException)
        ///					{
        ///						//在出错的情况下，通知池，该连接返回到池中时，应该被抛弃
        ///						conn.IsCorrupted = true;
        ///						throw;
        ///					}
        ///				}
        ///			}
        ///		}
        ///	</code>
        /// </example>
		public Pool(
            Func<T> itemFactory,
            Func<T, PoolItemPhase, bool> itemFilter,
            Action<T> itemDestroyer,
            PoolConfig config)
		{
			if (itemFactory == null) throw new ArgumentNullException("itemFactory");
			if (config == null) throw new ArgumentNullException("config");

			_itemFactory = itemFactory;
			_itemFilter = itemFilter;
            _itemDestroyer = itemDestroyer;

            _loanCapacity = config.LoanCapacity;
			_poolCapacity = config.PoolCapacity;
            _maxRemainTime = config.MaxRemainTime;
			_maxLifespan = config.MaxLifespan;

			_maxUses = config.MaxUses;
			_fetchOrder = config.FetchOrder;

			_container = _createContainer(config.PoolCapacity > 0 ? config.PoolCapacity : 10);
		}

        public Pool(
            Func<T> itemFactory,
            Func<T, PoolItemPhase, bool> itemFilter,
            PoolConfig config)
            : this(itemFactory, itemFilter, null, config)
        {
        }


        private IPoolContainer<ResidentItem> _createContainer(int capacity)
		{
			if (_fetchOrder == PoolFetchOrder.Fifo)
			{
				return new FifoContainer<ResidentItem>(capacity);
			}
			else if (_fetchOrder == PoolFetchOrder.Lifo)
			{
				return new LifoContainer<ResidentItem>(capacity);
			}
			else
			{
				throw new NotSupportedException(
					string.Format(Strings.NotSupportedPoolFetchOrder, _fetchOrder));
			}
		}

		/// <summary>
        /// 从池中借出项，当项使用完毕后，需要调用IPoolItem{T}.Dispose释放
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		///		<para>实例已被释放</para>
		/// </exception>
		/// <returns>
        /// 返回包含类型T的项实例，在交还给池之前，消费者可以使用它
		/// </returns>
		/// <example>
		/// 	<code>
		/// 	// 使用项:
		/// 	using(var borrowedItem = pool.Borrow())
		/// 	{
		/// 		// ... 使用 borrowedItem.Item
		/// 	}
		/// 	
		/// 	// 从池中移除项
		/// 	// 当项被抛弃时:
		/// 	
        /// 	// 使用项:
		/// 	using(var borrowedItem = pool.Borrow())
		/// 	{
		/// 		try
		/// 		{
        /// 			// ... 使用 borrowedItem.Item
		/// 		}
		/// 		catch(StateCorruptionException)
		/// 		{
		/// 			borrowedItem.IsCorrupted = true;
		/// 			throw;
		/// 		}
		/// 	}
		/// </code>
		/// </example>
		public IPoolItem<T> Borrow()
		{
			ResidentItem resident = null;
			List<ResidentItem> expiredItems = null;
			try
			{
				int currentPoolVersion;

				lock (_syncRoot)//当前线程进入就绪队列,争用锁
				{
					if (_isDisposed) throw new ObjectDisposedException(this.GetType().Name);

					currentPoolVersion = this._poolVersion; //借出时，池的版本号

                    while(BorrowedOverstep())
					{
                        //当借出的数量超过指定数量时，等待
						++_waiterCount;
                        //线程优先顺序： 【等待队列】->【就绪队列】->【拥有锁线程】
                        //同步的对象包含若干引用，其中包括对当前拥有锁的线程的引用、对就绪队列的引用和对等待队列的引用。
                        //竞争对象锁的线程都是处于就绪队列中。
                        Monitor.Wait(_syncRoot);//释放当前线程对_syncRoot的锁，流放当前线程到等待队列,消费者的线程此时会被阻塞
						--_waiterCount;
					}

					while (_container.Count > 0)
					{
						resident = _container.Take();
                        //如果项的寿命到期或者该项已被抛弃,后超过了停留时间(停留时间在每次使用后会被刷新)
                        if (IsExpired(resident) || !_filter(resident, PoolItemPhase.Leaving))
						{
							if (expiredItems == null) expiredItems = new List<ResidentItem>();
                            expiredItems.Add(resident);//那么将该项移到过期集合中
							resident = null;//重置项指针指向null
							continue;
						}
						break;
					}

					++_borrowedCount;
				}

				try
				{
                    //在容器中没有找到可用的项，那么创建被封装的新项
					if (resident == null) resident = new ResidentItem(this, _itemFactory());

					var borrowedItem = resident.Borrow(currentPoolVersion);

					if (borrowedItem == null)
                        throw new InvalidOperationException(Strings.PoolItemHaveBeenLent);

					return borrowedItem;
				}
				catch(Exception ex)
				{
                    DecrementBorrowedCount();//如果出错，则本次借出失败，减少一次借出的数量(因为之前++_borrowedCount)
                    throw ex;
				}
			}
			finally
			{
				if (expiredItems != null)
				{
					foreach (var item in expiredItems)
					{
						DiscardItem(item);
					}
				}
			}
		}

        /// <summary>
        /// 项是否过期
        /// </summary>
        /// <param name="resident"></param>
        /// <returns></returns>
        private bool IsExpired(ResidentItem resident)
        {
            return resident.IsCorrupted
                    || IsRemainTimeExpired(resident)
                    || IsLifespanExpired(resident)
                    || OverstepUses(resident);
        }


        /// <summary>
        /// 归还项
        /// </summary>
        /// <param name="item"></param>
        public void Return(IPoolItem<T> item)
        {
            item.Dispose();
        }

        /// <summary>
        /// 使用池中的项
        /// </summary>
        /// <param name="action"></param>
        public void Using(Action<T> action)
        {
            using (var item = this.Borrow())
            {
                try
                {
                    action(item.Item);
                }
                catch (Exception ex)
                {
                    item.IsCorrupted = true;
                    throw ex;
                }
            }
        }

		/// <summary>
		/// 	<para>获取池中当前项的数量</para>
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		///		<para>实例已被释放</para>
		/// </exception>
		public int Count
		{
			get
			{
				lock (_syncRoot)
				{
					if (_isDisposed) throw new ObjectDisposedException(this.GetType().Name);
					return _container.Count;
				}
			}
		}

        /// <summary>
        /// 清理池
        /// </summary>
		public void Clear()
		{
			IPoolContainer<ResidentItem> oldContainer;

			lock (_syncRoot)
			{
				if (_isDisposed) throw new ObjectDisposedException(this.GetType().Name);

				_poolVersion++; //更改池版本号，因此，借出的项在归还时，都会被清理

				oldContainer = _container;
				_container = _createContainer(_container.Count);
			}

			ClearPool(oldContainer);
		}

		private bool _filter(ResidentItem residentItem, PoolItemPhase phase)
		{
			if (_itemFilter == null) return true;
			try
			{
				return _itemFilter(residentItem.Item, phase);
			}
			catch (Exception ex)
			{
                throw new PoolingException(ex);
			}
		}

        /// <summary>
        /// 向池中归还项
        /// </summary>
        /// <param name="residentItem"></param>
		private void _return(ResidentItem residentItem)
		{
			DecrementBorrowedCount();

            //如果项被显示注明了需要抛弃
            //    或者项使用次数超过了限制
            //    或者项在池中的寿命超过了限制
            //那么项被需要被抛弃
            bool discard = IsExpired(residentItem);

			if (!discard) //如果项没有被抛弃，那么调用过滤方法，进一步判断，是否被抛弃
                discard = !_filter(residentItem, PoolItemPhase.Returning);

			if (discard)
			{
				DiscardItem(residentItem);
				return;
			}

			bool returned = false;
			lock (_syncRoot)
			{
				if (!_isDisposed
					&& residentItem.PoolVersionWhenBorrowed == this._poolVersion
					&& (_poolCapacity <= 0 || _container.Count < _poolCapacity))
				{
                    //如果池没有被释放
                    //    并且项的池版本等于当前池的版本
                    //    并且池中的项数量没有达到限定值
                    //那么将项放入容器中
					_container.Put(residentItem);
					returned = true;
				}
			}
			if (!returned) //如果没有返回，那么移除项
                DiscardItem(residentItem);
		}

        /// <summary>
        /// 减少借出数量，同时将等待队列中的一个线程放行到就绪队列中
        /// </summary>
		private void DecrementBorrowedCount()
		{
			lock (_syncRoot)
			{
				--_borrowedCount;
				if (_waiterCount > 0)
				{
                    //将等待队列中的一个线程放行到就绪队列
					Monitor.Pulse(_syncRoot);
				}
			}
		}

        /// <summary>
        /// 项的寿命是否到期
        /// </summary>
        /// <param name="residentItem"></param>
        /// <returns></returns>
		private bool IsLifespanExpired(ResidentItem residentItem)
		{
			return _maxLifespan > 0 && DateTime.UtcNow.Subtract(residentItem.UtcCreated).TotalSeconds >= _maxLifespan;
		}

        /// <summary>
        /// 超出使用次数
        /// </summary>
        /// <param name="residentItem"></param>
        /// <returns></returns>
        private bool OverstepUses(ResidentItem residentItem)
        {
            return _maxUses > 0 && residentItem.UseCount >= _maxUses;
        }

        /// <summary>
        /// 是否超过了停留时间
        /// </summary>
        /// <param name="residentItem"></param>
        /// <returns></returns>
        private bool IsRemainTimeExpired(ResidentItem residentItem)
        {
            return _maxRemainTime > 0 && DateTime.UtcNow.Subtract(residentItem.UtcLastUsed).TotalSeconds >= _maxRemainTime;
        }

        /// <summary>
        /// 抛弃并释放池中的项
        /// </summary>
        /// <param name="residentItem"></param>
		private void DiscardItem(ResidentItem residentItem)
        {
            try
            {
                if (_itemDestroyer != null)
                    _itemDestroyer(residentItem.Item);


                var disposableObject = residentItem.Item as IDisposable;
                if (disposableObject != null) disposableObject.Dispose();
            }
            catch (Exception ex)
            {
                throw new PoolingException(string.Format(Strings.DisposePoolItemFailed, typeof(Pool<T>)), ex);
            }
        }

		/// <summary>
        /// 清理池中所有项，并且标示池已被释放，不能再使用
		/// </summary>
		public void Dispose()
		{
			lock (_syncRoot)
			{
				_isDisposed = true;

				ClearPool(_container);
			}
		}

        /// <summary>
        /// 实现真正清理池的动作
        /// </summary>
        /// <param name="container"></param>
		private void ClearPool(IPoolContainer<ResidentItem> container)
		{
			while (container.Count > 0)
			{
				var item = container.Take();
				DiscardItem(item);
			}
        }

        #region 重构

        
        public bool BorrowedOverstep()
        {
            return _loanCapacity > 0 && _borrowedCount >= _loanCapacity;
        }

        /// <summary>
        /// 借出的数量超过最大值
        /// </summary>
        /// <returns></returns>
        public bool IsBorrowedOverstep
        {
            get
            {
                lock (_syncRoot)
                {
                    return this.BorrowedOverstep();
                }
            }
        }

        #endregion

    }
}
