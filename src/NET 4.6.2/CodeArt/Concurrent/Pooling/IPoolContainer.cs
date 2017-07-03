using System;

namespace CodeArt.Concurrent
{
    internal interface IPoolContainer<TItem>
    {
        /// <summary>
        /// 获取项
        /// </summary>
        /// <returns></returns>
        TItem Take();
        void Put(TItem item);
        int Count { get; }
    }
}
