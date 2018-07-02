using System;
using System.Collections.Generic;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// 先进先出的容器
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    internal class FifoContainer<TItem> : IPoolContainer<TItem>
    {
        private readonly Queue<TItem> _queue;

        public FifoContainer(int capacity)
        {
            _queue = new Queue<TItem>(capacity);
        }

        #region IContainer 成员

        TItem IPoolContainer<TItem>.Take()
        {
            //取出时，取的是顶部对象（顶部对象是在容器中呆的比较久的对象）
            return _queue.Dequeue();
        }

        void IPoolContainer<TItem>.Put(TItem item)
        {
            //新来的对象，放在结尾处，这样顶部对象就是呆的比较久的对象
            _queue.Enqueue(item);
        }

        int IPoolContainer<TItem>.Count
        {
            get { return _queue.Count; }
        }

        #endregion
    }

}
