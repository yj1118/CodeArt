using System;
using System.Linq;

namespace CodeArt.Concurrent
{
    /// <summary>
    /// future状态
    /// </summary>
    public enum FutureStatus
    {
        /// <summary>
        /// 实例未完成
        /// </summary>
        Incomplete,
        /// <summary>
        /// 成功完成
        /// </summary>
        Success,
        /// <summary>
        /// 已完成，但是未成功，有错误
        /// </summary>
        Failure,
        /// <summary>
        /// 已被取消
        /// </summary>
        Canceled,
        /// <summary>
        /// 初始状态
        /// </summary>
        Initial
    }
}
