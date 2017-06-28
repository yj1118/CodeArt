using System;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 	<para>一个简单的，用于方法调用中参数的 <see cref="IVariable"/>实现</para>
    /// </summary>
    internal class SimpleParameter : SimpleVariable
    {
        private static Type GetElementType(Type type)
        {
            return type.IsByRef ? type.GetElementType() : type;
        }

        private static string GetName(string name, int index)
        {
            return string.IsNullOrEmpty(name)
                ? "(parameter[" + index + "])"
                : name;
        }

        private readonly Type _originalType;

        public SimpleParameter(ICILWriter writer, int index, Type type, string name)
            : base(writer, index, GetElementType(type), GetName(name, index), false)
        {
            _originalType = type;
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="options"></param>
        public override void Load(LoadOptions options)
        {
            if (_originalType.IsByRef)
            {
                Writer.EmitLdarg(Index);
                if (!options.ShouldLoadAddress(Type))
                {
                    Writer.EmitLdobj(Type);
                }
            }
            else
            {
                if (options.ShouldLoadAddress(_originalType))
                {
                    Writer.EmitLdarga(Index);
                }
                else
                {
                    Writer.EmitLdarg(Index);
                }
            }
        }

        /// <summary>
        /// Emits the necessary instructions to store to this variable.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="CanStore"/> is <see langword="false"/>.</exception>
        public override void Store()
        {
            if (!CanStore)
            {
                throw new InvalidOperationException("Can't store when Type is a ByRef type.");
            }
            Writer.EmitStarg(Index);
        }

        /// <summary>
        /// Gets a value indicating whether this instance can use the <see cref="Store"/> method.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can use the <see cref="Store"/> method; otherwise, <c>false</c>.
        /// </value>
        public override bool CanStore
        {
            get { return !_originalType.IsByRef; }
        }

        /// <summary>
        /// 	<para>发出赋值给该变量的指令块的开始</para>
        /// 	<para>需要在<see cref="BeginAssign"/> 和 <see cref="EndAssign"/>之间加载需要赋值的值</para>
        /// </summary>
        public override void BeginAssign()
        {
            if (_originalType.IsByRef)
            {
                Writer.EmitLdarg(Index);
            }
        }

        /// <summary>
        /// 	<para>发出指令块分配给该变量的结尾。</para>
        /// </summary>
        public override void EndAssign()
        {
            if (_originalType.IsByRef)
            {
                Writer.EmitStobj(Type);
            }
            else
            {
                Writer.EmitStarg(Index);
            }
        }
    }
}
