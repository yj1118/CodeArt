using System;

namespace CodeArt.Concurrent
{
	/// <summary>
    ///	<para>指示项是进入还是离开池.</para>
	/// </summary>
	public enum PoolItemPhase
	{
        /// <summary>
        /// 表示IPoolItem{T}已离开Pool{T},离开是指移除，不是借出去
        /// </summary>
		Leaving,

		/// <summary>
        ///	<para>表示IPoolItem{T}已回到Pool{T}</para>
		/// </summary>
		Returning
	}
}
