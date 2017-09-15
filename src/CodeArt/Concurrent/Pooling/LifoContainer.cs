using System;
using System.Collections.Generic;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// 后进先出的容器
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    internal class LifoContainer<TItem> : IPoolContainer<TItem>
    {
        private readonly Stack<TItem> _stack;

        public LifoContainer(int capacity)
        {
            _stack = new Stack<TItem>(capacity);
        }

        #region IContainer Members

        TItem IPoolContainer<TItem>.Take()
        {
            //取出时，取的是顶部对象（顶部对象是新进来的对象）
            return _stack.Pop();
        }

        void IPoolContainer<TItem>.Put(TItem item)
        {
            //新来的对象，放在顶部，这样顶部对象就是最新进来的对象，底部的对象是呆的比较久的对象
            _stack.Push(item);
        }

        int IPoolContainer<TItem>.Count
        {
            get { return _stack.Count; }
        }

        #endregion
    }


}
