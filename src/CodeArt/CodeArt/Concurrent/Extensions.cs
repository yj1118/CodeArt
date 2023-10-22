using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

using CodeArt.Log;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// 常用的扩展
    /// </summary>
    public static class Extensions
    {
        #region 读写锁

        public static void Read(this ReaderWriterLockSlim rw, Action action)
        {
            rw.EnterReadLock();
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                throw ex;
            }
            finally
            {
                rw.ExitReadLock();
            }
        }

        public static void Write(this ReaderWriterLockSlim rw, Action action)
        {
            rw.EnterWriteLock();
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                throw ex;
            }
            finally
            {
                rw.ExitWriteLock();
            }
        }

        #endregion

        #region 并发对象

        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            T item;
            while (queue.TryDequeue(out item))
            {
            }
        }

        public static T Dequeue<T>(this ConcurrentQueue<T> queue)
        {
            T item;
            queue.TryDequeue(out item);
            return item;
        }

        public static bool TryUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue newValue)
        {
            TValue oldValue = default(TValue);
            if (dictionary.TryGetValue(key, out oldValue))
            {
                //运行到此处，如果有其他的线程将key删除了，那么下面的代码会返回false，所以不算bug
                //因此依然保证了多线程访问是返回正确的结果
                return dictionary.TryUpdate(key, newValue, oldValue);
            }
            return false;
        }

        #endregion

        #region 缓存常用

        /// <summary>
        /// 利用ditionary对象做缓存用，会经常用到该方法
        /// 该方法内部使用双锁的模式确保并发的正确
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> create)
        {
            TValue value = default(TValue);
            if (!dictionary.TryGetValue(key, out value))
            {
                lock (dictionary)
                {
                    if (!dictionary.TryGetValue(key, out value))
                    {
                        value = create(key);
                        dictionary.Add(key, value);
                    }
                }
            }
            return value;
        }


        #endregion

    }
}
