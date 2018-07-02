using System;

namespace CodeArt.Concurrent
{
	/// <summary>
    ///	从Pool{T}实例中检索项的方式
	/// </summary>
	public enum PoolFetchOrder
	{
		/// <summary>
        /// 就像队列那样,使用先入先出的算法
		/// </summary>
		Fifo,
		/// <summary>
        /// 就像堆栈那样，使用后入先出的算法
		/// </summary>
		Lifo
	}
}
