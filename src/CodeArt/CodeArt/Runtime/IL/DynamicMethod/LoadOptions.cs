using System;
using System.Linq;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 表示以什么方式加载<see cref="IVariable"/> 或 <see cref="IValueShortcut" />
    /// </summary>
    [Flags]
    public enum LoadOptions
    {
        /// <summary>
        ///	<para>值类型:加载值到堆栈</para>
        ///	<para>或</para>
        ///	<para>引用类型:加载引用指针到堆栈</para>
        /// </summary>
        Default = 0,
        /// <summary>
        /// 如果启用，那么加载值类型变量的内存地址
        /// </summary>
        ValueAsAddress = 1 << 1,
        /// <summary>
        /// 如果启用，那么加载引用指针的内存地址
        /// </summary>
        ReferenceAsAddress = 1 << 2,
        /// <summary>
        /// 如果启用，表示类型可以被装箱
        /// </summary>
        BoxValues = 1 << 3,
        /// <summary>
        /// 同时设置<see cref="ValueAsAddress"/> 和 <see cref="ReferenceAsAddress"/>标记
        /// </summary>
        AnyAsAddress = ValueAsAddress | ReferenceAsAddress
    }
}
