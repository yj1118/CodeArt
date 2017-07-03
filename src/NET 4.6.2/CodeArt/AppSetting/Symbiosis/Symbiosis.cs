using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Text;
using System.Diagnostics;

using CodeArt.Runtime;
using CodeArt.AppSetting;
using CodeArt.Concurrent;


namespace CodeArt.AppSetting
{
    /// <summary>
    /// 共生器
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public class Symbiosis : IDisposable
    {
        /// <summary>
        /// 打开共生器的次数
        /// </summary>
        internal int OpenTimes
        {
            get;
            private set;
        }

        private SymbioticCollection _symbiotics;

        public int Count
        {
            get
            {
                return _symbiotics.Count;
            }
        }


        private Symbiosis()
        {
            this.OpenTimes = 0;
            _symbiotics = new SymbioticCollection();
        }
     
        public void Clear()
        {
            _symbiotics.Clear();
        }


        /// <summary>
        /// 标记对象是与当前数据上下文是共生的，当数据上下文被释放时，会销毁该对象
        /// 该方法很适合与对象池机制连用，达到数据上下文使用完毕后，相关的对象也被归还到池中
        /// </summary>
        /// <param name="item"></param>
        private void Mark(IDisposable item)
        {
            _symbiotics.Add(item);
        }

        public void Dispose()
        {
            this.Clear();
        }


        #region 基于当前应用程序回话的数据上下文


        private const string _sessionKey = "__Symbiosis.Current";

        /// <summary>
        /// 获取或设置当前会话的数据上下文
        /// </summary>
        public static Symbiosis Current
        {
            get
            {
                var current = AppSession.GetItem<Symbiosis>(_sessionKey);
                if (current == null) throw new AppSettingException(Strings.NoSymbiosis);
                return current;
            }
            private set
            {
                AppSession.SetItem<Symbiosis>(_sessionKey, value);
            }
        }

        /// <summary>
        /// 当前应用程序会话中是否存在共生器
        /// </summary>
        /// <returns></returns>
        private static bool ExistsCurrent()
        {
            return AppSession.Exists() && AppSession.GetItem<Symbiosis>(_sessionKey) != null;
        }

        /// <summary>
        /// 尝试标记对象，如果共生器不存在，那么使用<paramref name="creator"/>创建一个全新的对象
        /// 如果共生器存在，那么使用<paramref name="pool"/>池借对象，当共生器被销毁时，借出的项会被返回到池中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pool"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static T TryMark<T>(Pool<T> pool, Func<T> creator)
        {
            if (ExistsCurrent())
            {
                var temp = pool.Borrow();
                Current.Mark(temp);
                return temp.Item;
            }
            else
                return creator();
        }


        #endregion

        #region 对象池


        private static PoolWrapper<Symbiosis> _pool;

        /// <summary>
        /// 打开共生器
        /// </summary>
        public static Symbiosis Open()
        {
            Symbiosis sym;
            if (ExistsCurrent())
            {
                sym = Current;
            }
            else
            {
                sym = _pool.Borrow();
                Current = sym;
            }
            sym.OpenTimes++;
            return sym;
        }

        /// <summary>
        /// 关闭当前共生器
        /// </summary>
        public static void Close()
        {
            Symbiosis sym = Current;
            sym.OpenTimes--;
            if (sym.OpenTimes == 0)
            {
                _pool.Return(sym);
                Current = null;
            }
        }


        static Symbiosis()
        {
            _pool = new PoolWrapper<Symbiosis>(() =>
            {
                return new Symbiosis();
            }, (sym, phase) =>
            {
                if(phase == PoolItemPhase.Returning)
                {
                    sym.Clear();
                }
                return true;
            }, new PoolConfig()
            {
                MaxRemainTime = 300 //5分钟之内未被使用，就移除
            });
        }

        #endregion

    }
}
