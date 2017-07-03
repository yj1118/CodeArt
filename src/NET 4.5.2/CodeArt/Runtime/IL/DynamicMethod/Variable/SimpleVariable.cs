using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using CodeArt.Util;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 	<para>一个简单、抽象的<see cref="IVariable"/>实现</para>
    /// 	<para>这个类是拟用于内部，由<see cref="ICILWriter"/>实现。</para>
    /// </summary>
    internal abstract class SimpleVariable : IVariable
    {
        protected SimpleVariable(ICILWriter writer, int index, Type type, string name, bool isPinned)
        {
            ArgumentAssert.IsNotNull(writer, "writer");
            ArgumentAssert.IsNotNull(type, "type");

            Writer = writer;
            Index = index;
            Type = type;
            Name = name;
            IsPinned = isPinned;
        }

        #region IVariable Members

        protected ICILWriter Writer { get; private set; }


        /// <summary>
        /// <pra>变量序号</pra>
        /// <para>局部变量时，为局部变量的序号；参数时，是参数的序号...</para>
        /// </summary>
        protected int Index { get; private set; }


        /// <summary>
        /// 获取变量类型
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 变量名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 变量是否为固定的
        /// </summary>
        public bool IsPinned { get; private set; }

        /// <summary>
        /// <para>初始化或者重置变量的值，例如：值类型的变量重置为0，引用类型的重置为null等</para>
        /// </summary>
        public void Initialize()
        {
            if (Type.IsPrimitive)
            {
                var size = Type.GetPrimitiveSize();
                if (Type.IsFloatingPoint())
                {
                    if (size == 4)
                    {
                        Writer.Emit(OpCodes.Ldc_R4, 0f);
                        Store();
                        return;
                    }
                    if (size == 8)
                    {
                        Writer.Emit(OpCodes.Ldc_R8, 0d);
                        Store();
                        return;
                    }
                }
                else
                {
                    if (size <= 4)
                    {
                        Writer.EmitLdcI4(0);
                        Store();
                        return;
                    }
                    if (size == 8)
                    {
                        Writer.Emit(OpCodes.Ldc_I8, 0L);
                        Store();
                        return;
                    }
                }
            }

            if (Type.IsValueType || Type.IsGenericParameter)
            {
                Load(LoadOptions.AnyAsAddress);
                Writer.Emit(OpCodes.Initobj, Type);
            }
            else
            {
                Writer.Emit(OpCodes.Ldnull);
                Store();
            }
        }

        /// <summary>
        /// 发出指令：加载变量的值到计算堆栈
        /// </summary>
        public abstract void Load(LoadOptions options);

        /// <summary>
        /// 发出指令：从计算堆栈中取出值，存入到变量中
        /// </summary>
        public abstract void Store();

        /// <summary>
        /// 获取一个值，指示该变量是否可以使用<see cref="Store"/>方法
        /// </summary>
        public virtual bool CanStore
        {
            get { return true; }
        }

        /// <summary>
        /// 	<para>Emits the start of the instruction block for an assignment to this variable.</para>
        /// 	<para>The value to assign should be loaded between calls to <see cref="BeginAssign"/> and <see cref="EndAssign"/>.</para>
        /// </summary>
        public virtual void BeginAssign()
        {
        }

        /// <summary>
        /// 	<para>Emits the end of the instruction block for an assignment to this variable.</para>
        /// 	<para>The value to assign should be loaded between calls to <see cref="BeginAssign"/> and <see cref="EndAssign"/>.</para>
        /// </summary>
        public virtual void EndAssign()
        {
            Store();
        }

        #endregion
    }
}
