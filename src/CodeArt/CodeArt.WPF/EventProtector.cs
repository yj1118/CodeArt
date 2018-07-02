using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CodeArt.WPF
{
    /// <summary>
    /// 该对象用于防止快速点击导致的重复响应事件的问题
    /// </summary>
    public class EventProtector
    {
        private int _ing = 0;

        public EventProtector()
        {
        }

        /// <summary>
        /// 开始执行事件
        /// </summary>
        public void Start(Action action)
        {
            if (Interlocked.CompareExchange(ref _ing, 1, 0) == 0)
            {
                action();
            }
        }

        /// <summary>
        /// 事件执行结束
        /// </summary>
        public void End()
        {
            Interlocked.CompareExchange(ref _ing, 0, 1);
        }
    }


    public class EventProtector<T>
    {
        private int _ing = 0;

        public EventProtector()
        {
        }

        public void Start(Action<T> action, T arg)
        {
            if (Interlocked.CompareExchange(ref _ing, 1, 0) == 0)
            {
                action(arg);
            }
        }

        /// <summary>
        /// 事件执行结束
        /// </summary>
        public void End()
        {
            Interlocked.CompareExchange(ref _ing, 0, 1);
        }
    }
}
