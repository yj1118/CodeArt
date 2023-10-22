using CodeArt.Concurrent.Pattern;
using CodeArt.Log;
using CodeArt.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CodeArt.Web
{
    public static class WorkSession
    {
        private static ConcurrentDictionary<Guid, Item> _items = new ConcurrentDictionary<Guid, Item>();

        private static Timer _timer;

        static WorkSession()
        {
            InitTimer();
        }

        public static void Set(Guid workId, object value)
        {
            _items.AddOrUpdate(workId, (id) =>
            {
                return new Item(value, false, null);
            }, (id, old) =>
            {
                old.SetValue(value);
                return old;
            });
        }

        public static void SetException(Guid workId, Exception ex)
        {
            _items.AddOrUpdate(workId, (id) =>
            {
                return new Item(null, false, ex);
            }, (id, old) =>
            {
                old.SetException(ex);
                return old;
            });
        }

        public static void Cancel(Guid workId)
        {
            _items.AddOrUpdate(workId, (id) =>
            {
                return new Item(null, true, null);
            }, (id, old) =>
            {
                old.Cancel(true);
                return old;
            });
        }

        public static bool IsCancelled(Guid workId)
        {
            Item item = null;
            _items.TryGetValue(workId, out item);
            if (item != null)
            {
                return item.IsCancelled;
            }
            return false; //没有选项的时候，也不表示取消了
        }

        public static T Get<T>(Guid workId, T defaultValue)
        {
            Item item = null;
            _items.TryGetValue(workId, out item);
            if (item != null)
            {
                if (item.Exception != null) throw item.Exception;
                return (T)item.GetValue();
            }
            return defaultValue;
        }

        public static void Remove(Guid workId)
        {
            _items.TryRemove(workId, out var item);
        }

        /// <summary>
        /// 清理过期项
        /// </summary>
        private static void Clear()
        {
            List<Guid> expireds = new List<Guid>();
            foreach (var p in _items)
            {
                if (p.Value.IsExpired)
                {
                    expireds.Add(p.Key);
                }
            }

            foreach (var e in expireds)
            {
                _items.TryRemove(e, out var item);
            }
        }


        #region 定时清理

        private static void InitTimer()
        {
            _timer = new Timer(5 * 60 * 1000); //每间隔5分钟执行一次
            _timer.Elapsed += OnElapsed;
            _timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)
            _timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
        }

        private static void OnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Clear();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
            finally
            {
                _timer.Start();
            }
        }

        #endregion

        /// <summary>
        /// 回话项
        /// </summary>
        private sealed class Item
        {
            public object Value
            {
                get;
                private set;
            }

            /// <summary>
            /// 获取值也会更新时间
            /// </summary>
            public object GetValue()
            {
                this.LastAccessTime = DateTime.Now;
                return this.Value;
            }

            public void SetValue(object value)
            {
                this.Value = value;
                this.LastAccessTime = DateTime.Now;
            }

            /// <summary>
            /// 最后一次访问时间
            /// </summary>
            public DateTime LastAccessTime
            {
                get;
                private set;
            }

            public bool IsExpired
            {
                get
                {
                    return this.LastAccessTime.AddMinutes(5) < DateTime.Now;  //5分钟之内没有访问，那回话项就是过期了
                }
            }

            public bool IsCancelled
            {
                get;
                private set;
            }

            public void Cancel(bool isCancelled)
            {
                this.IsCancelled = isCancelled;
                this.LastAccessTime = DateTime.Now;
            }

            public Exception Exception
            {
                get;
                private set;
            }

            public void SetException(Exception ex)
            {
                this.Exception = ex;
                this.LastAccessTime = DateTime.Now;
            }

            public Item(object value, bool isCancelled, Exception ex)
            {
                if (value != null)
                    this.SetValue(value);

                this.Cancel(isCancelled);

                if (ex != null)
                    this.SetException(ex);
            }

        }
    }
}