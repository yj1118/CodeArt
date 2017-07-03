using System;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 	<para>A simple, general-purpose implementation of <see cref="IVariable"/>.
    /// 	This class is intended to be used internally by <see cref="ICILWriter"/> implementations.</para>
    /// </summary>
    internal class SimpleLocal : SimpleVariable
    {
        /// <summary>
        /// 	<para>Initializes a new instance of the <see cref="SimpleLocal"/> class.</para>
        /// </summary>
        /// <param name="writer">The writer that the parameter belongs to.</param>
        /// <param name="index">The index of the parameter.</param>
        /// <param name="type">The type of the local variable.</param>
        /// <param name="name">The name of the local variable.</param>
        /// <param name="isPinned">A value indicating whether or not the local variable is pinned.</param>
        /// <exception cref="ArgumentNullException">
        ///	<para><paramref name="writer"/> is <see langword="null"/>.</para>
        ///	<para>- or -</para>
        ///	<para><paramref name="type"/> is <see langword="null"/>.</para>
        /// </exception>
        public SimpleLocal(ICILWriter writer, int index, Type type, string name, bool isPinned)
            : base(writer, index, type, name, isPinned)
        {
            if (type.IsByRef) throw new NotSupportedException("ByRef types are not supported by this class.");
        }

        /// <summary>
        /// 加载变量到计算堆栈上
        /// </summary>
        public override void Load(LoadOptions options)
        {
            if (options.ShouldLoadAddress(Type))
            {
                Writer.EmitLdloca(Index);
            }
            else
            {
                Writer.EmitLdloc(Index);
            }
        }

        /// <summary>
        /// Emits the necessary instructions to store to this variable.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="IVariable.CanStore"/> is <see langword="false"/>.</exception>
        public override void Store()
        {
            Writer.EmitStloc(Index);
        }
    }
}
