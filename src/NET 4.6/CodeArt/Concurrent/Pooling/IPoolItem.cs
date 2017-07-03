using System;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// 一个属于Pool{T}实例的项
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
	public interface IPoolItem<T> : IDisposable
	{
		/// <summary>
        /// 获取项所属的Pool{T}实例
		/// </summary>
		Pool<T> Owner { get; }

        /// <summary>
        /// 获取项
        /// </summary>
		T Item { get; }

		/// <summary>
        /// 获取或设置一个值，表示该项是否是损坏的，如果是损坏的，那么应该从它所属的Pool{T}实例中将该项移除
		/// </summary>
		bool IsCorrupted { get; set; }
	}
}
