using System;
using System.Reflection.Emit;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 变量，局部变量、参数都是变量的实现
    /// </summary>
    public interface IVariable
    {
        /// <summary>
        /// 获取变量类型
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// <para>获取一个值，指示变量是否为pinned(固定的)</para>
        /// <para>Pinned修饰符只适用于本地变量。它的使用意味着由本地变量引用的对象不可以被垃圾收集重新部署并且必须贯穿方法执行是原位不动的</para>
        /// <para>当引用对象是固定的，那么它不能被垃圾回收移动。</para>
        /// </summary>
        bool IsPinned { get; }

        /// <summary>
        /// 变量名称，当变量为nul
        /// 该属性无效
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     <para>使用<see cref="OpCodes.Initobj"/>操作码初始化或重置变量的值为空引用或基元类型的 0</para>
        ///     <para>将位于指定地址的对象的所有字段初始化为空引用或基元类型的 0。</para>
        /// </summary>
        void Initialize();

        /// <summary>
        /// 	<para>发出一个指令，加载变量的值到堆栈</para>
        /// </summary>
        void Load(LoadOptions options);

        /// <summary>
        ///		<para>发出存储变量的指令</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///		<para><see cref="CanStore"/> 是 <see langword="false"/>.</para>
        /// </exception>
        void Store();

        /// <summary>
        /// 获取一个值，指示实例是否能够使用<see cref="Store"/> 方法
        /// </summary>
        bool CanStore { get; }

        /// <summary>
        /// 	<para>发出赋值给该变量的指令块的开始</para>
        /// 	<para>需要在<see cref="BeginAssign"/> 和 <see cref="EndAssign"/>之间加载需要赋值的值</para>
        /// </summary>
        void BeginAssign();

        /// <summary>
        /// 	<para>发出指令块分配给该变量的结尾。</para>
        /// </summary>
        void EndAssign();
    }
}
