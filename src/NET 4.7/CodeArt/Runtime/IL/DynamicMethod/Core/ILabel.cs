using System;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 	<para>封装了一个标签，可以被标记和引用，由<see cref="IMsilWriter"/>实现.</para>
    /// </summary>
    public interface ILabel
    {
        /// <summary>
        /// 将MSIL流的当前位置标记给该标签
        /// </summary>
        void Mark();

        /// <summary>
        /// 	<para>获取一个值，指示实例是否被一些分支指令引用</para>
        /// </summary>
        /// <value>
        /// 	<para><see langword="true"/> 实例被引用; 否则, <see langword="false"/>.</para>
        /// </value>
        bool IsReferenced { get; }

        /// <summary>
        /// 获取一个值，表示该标签是否被标记了MSIL流位置
        /// </summary>
        bool IsMarked { get; }
    }
}
