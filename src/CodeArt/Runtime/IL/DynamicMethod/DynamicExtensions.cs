using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;


using CodeArt.Util;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 为动态代码封装了扩展方法
    /// </summary>
    internal static class DynamicExtensions
    {
        /// <summary>
        /// <para>加载数组元素的操作码</para>
        /// <para>需要加载的元素类型->操作码</para>
        /// </summary>
        private static readonly Dictionary<Type, OpCode> _primitiveLdelemOpCodes;
        private static readonly Dictionary<Type, OpCode> _primitiveStelemOpCodes;
        private static readonly Dictionary<Type, OpCode> _primitiveLdobjOpCodes;
        private static readonly Dictionary<Type, OpCode> _primitiveStobjOpCodes;
        private static readonly Dictionary<Type, int> _primitiveSizes = new Dictionary<Type, int>
		{
			{ typeof(bool), 1 },
			{ typeof(char), 1 },
			{ typeof(byte), 1 },
			{ typeof(sbyte), 1 },
			{ typeof(short), 2 },
			{ typeof(ushort), 2 },
			{ typeof(int), 4 },
			{ typeof(uint), 4 },
			{ typeof(long), 8 },
			{ typeof(ulong), 8 },
			{ typeof(float), 4},
			{ typeof(double), 8},
			{ typeof(decimal), 16}
		};

        static DynamicExtensions()
        {
            #region 收集类型对应的加载数组元素的操作码
            _primitiveLdelemOpCodes = new Dictionary<Type, OpCode>
            {
                { typeof(sbyte), OpCodes.Ldelem_I1 },
                { typeof(short), OpCodes.Ldelem_I2 },
                { typeof(int), OpCodes.Ldelem_I4 },
                { typeof(long), OpCodes.Ldelem_I8 },
                { typeof(float), OpCodes.Ldelem_R4 },
                { typeof(double), OpCodes.Ldelem_R8 },
                { typeof(byte), OpCodes.Ldelem_U1 },
                { typeof(ushort), OpCodes.Ldelem_U2 },
                { typeof(uint), OpCodes.Ldelem_U4 }
            };
            #endregion

            _primitiveStelemOpCodes = new Dictionary<Type, OpCode>
            {
                { typeof(sbyte), OpCodes.Stelem_I1 },
                { typeof(short), OpCodes.Stelem_I2 },
                { typeof(int), OpCodes.Stelem_I4 },
                { typeof(long), OpCodes.Stelem_I8 },
                { typeof(float), OpCodes.Stelem_R4 },
                { typeof(double), OpCodes.Stelem_R8 }
            };
            _primitiveLdobjOpCodes = new Dictionary<Type, OpCode>
			{
                { typeof(sbyte), OpCodes.Ldind_I1 },
                { typeof(short), OpCodes.Ldind_I2 },
                { typeof(int), OpCodes.Ldind_I4 },
                { typeof(long), OpCodes.Ldind_I8 },
                { typeof(float), OpCodes.Ldind_R4 },
                { typeof(double), OpCodes.Ldind_R8 },
                { typeof(byte), OpCodes.Ldind_U1 },
                { typeof(ushort), OpCodes.Ldind_U2 },
                { typeof(uint), OpCodes.Ldind_U4 }
			};
            _primitiveStobjOpCodes = new Dictionary<Type, OpCode>
			{
                { typeof(sbyte), OpCodes.Stind_I1 },
                { typeof(short), OpCodes.Stind_I2 },
                { typeof(int), OpCodes.Stind_I4 },
                { typeof(long), OpCodes.Stind_I8 },
                { typeof(float), OpCodes.Stind_R4 },
                { typeof(double), OpCodes.Stind_R8 }
			};
        }

        /// <summary>
        /// <para>根据options设置
        /// 返回表示作为 ref 参数（在 Visual Basic 中为 ByRef 参数）传递时的当前类型的 System.Type 对象。
        /// </para>
        /// <para>
        /// 如果options没有设置成ValueAsAddress（按值地址传参）或ReferenceAsAddress（按引用地址传参）
        /// 那么类型不能作为ref参数，因此返回类型本身
        /// </para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static Type ResolveByRef(this Type type, LoadOptions options)
        {
            ArgumentAssert.IsNotNull(type, "type");

            if (type.IsValueType)
            {
                if (options.IsSet(LoadOptions.ValueAsAddress))
                    return type.ResolveByRef();
            }
            else
            {
                if (options.IsSet(LoadOptions.ReferenceAsAddress))
                    return type.ResolveByRef();
            }
            return type;
        }

     

        /// <summary>
        /// 检查是否设置了输入的选项
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool IsSet(this LoadOptions options, LoadOptions value)
        {
            return (options & value) == value;
        }

        internal static void EmitLdobj(this ICILWriter writer, Type type)
        {
            OpCode opCode;
            if (type.IsPrimitive && _primitiveLdobjOpCodes.TryGetValue(type, out opCode))
            {
                writer.Emit(opCode);
                return;
            }

            if (type.IsValueType || type.IsGenericParameter)
            {
                writer.Emit(OpCodes.Ldobj, type);
            }
            else
            {
                writer.Emit(OpCodes.Ldind_Ref);
            }
        }

        internal static void EmitStobj(this ICILWriter writer, Type type)
        {
            OpCode opCode;
            if (type.IsPrimitive && _primitiveStobjOpCodes.TryGetValue(type, out opCode))
            {
                writer.Emit(opCode);
                return;
            }

            if (type.IsValueType || type.IsGenericParameter)
            {
                writer.Emit(OpCodes.Stobj, type);
            }
            else
            {
                writer.Emit(OpCodes.Stind_Ref);
            }
        }

        /// <summary>
        /// <para>将指定数组索引中的元素加载到计算堆栈的顶部</para>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementType">元素类型</param>
        internal static void EmitLdelem(this ICILWriter writer, Type elementType)
        {
            OpCode opCode;
            if (elementType.IsPrimitive && _primitiveLdelemOpCodes.TryGetValue(elementType, out opCode))
            {
                writer.Emit(opCode);
                return;
            }

            if (elementType.IsValueType || elementType.IsGenericParameter)
            {
                writer.Emit(OpCodes.Ldelem, elementType);
            }
            else
            {
                writer.Emit(OpCodes.Ldelem_Ref);
            }
        }

        /// <summary>
        /// 用计算堆栈中的值替换给定索引处的数组元素，其类型在指令中指定。
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementType"></param>
        internal static void EmitStelem(this ICILWriter writer, Type elementType)
        {
            OpCode opCode;
            if (elementType.IsPrimitive && _primitiveStelemOpCodes.TryGetValue(elementType, out opCode))
            {
                writer.Emit(opCode);
                return;
            }

            if (elementType.IsValueType || elementType.IsGenericParameter)
            {
                writer.Emit(OpCodes.Stelem, elementType);
            }
            else
            {
                writer.Emit(OpCodes.Stelem_Ref);
            }
        }

        internal static ItemType GetItemType(this LoadOptions loadOptions, Type targetType)
        {
            return loadOptions.GetItemType(targetType, false);
        }

        internal static ItemType GetItemType(this LoadOptions loadOptions, Type targetType, bool boxed)
        {
            if (targetType.IsValueType && !boxed)
            {
                return loadOptions.IsSet(LoadOptions.ValueAsAddress)
                    ? ItemType.AddressToValue
                    : ItemType.Value;
            }

            return loadOptions.IsSet(LoadOptions.ReferenceAsAddress)
                ? ItemType.AddressToReference
                : ItemType.Reference;
        }

        /// <summary>
        /// 检查类型在该LoadOptions设置下是否可以装箱
        /// </summary>
        /// <param name="loadOptions"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static bool ShouldBox(this LoadOptions loadOptions, Type targetType)
        {
            ArgumentAssert.IsNotNull(targetType, "targetType");

            return targetType.IsValueType && loadOptions.IsSet(LoadOptions.BoxValues);
        }

        internal static bool ShouldLoadAddress(this LoadOptions loadOptions, Type targetType)
        {
            ArgumentAssert.IsNotNull(targetType, "targetType");

            if (targetType.IsValueType)
            {
                return (loadOptions & LoadOptions.ValueAsAddress) == LoadOptions.ValueAsAddress;
            }
            return (loadOptions & LoadOptions.ReferenceAsAddress) == LoadOptions.ReferenceAsAddress;
        }


        public static IVariable DefineLocal(this ICILWriter writer, Type type, bool isPinned, string name)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            return writer.DefineLocal(type, isPinned, name);
        }

        public static IExpression AddMember(this IExpression valueShortcut, MemberInfo member)
        {
            ArgumentAssert.IsNotNull(member, "member");

            if (member is PropertyInfo)
            {
                return valueShortcut.AddMember((PropertyInfo)member);
            }
            if (member is FieldInfo)
            {
                return valueShortcut.AddMember((FieldInfo)member);
            }
            throw new ArgumentException("member is not a PropertyInfo or a MemberInfo", "member");
        }

        /// <summary>
        /// Emits <see langword="OpCodes.Starg"/> or the best alternative macro depending on the operand.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="argumentIndex">The index of the parameter operand.</param>
        public static void EmitStarg(this ICILWriter writer, int argumentIndex)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (argumentIndex < 0) throw new ArgumentOutOfRangeException("argumentIndex", "argumentIndex may not be negative");

            if (argumentIndex <= byte.MaxValue)
            {
                writer.Emit(OpCodes.Starg_S, (byte)argumentIndex);
                return;
            }
            writer.Emit(OpCodes.Starg, argumentIndex);
        }

        /// <summary>
        /// Emits <see langword="OpCodes.Ldarga"/> or the best alternative macro depending on the operand.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="argumentIndex">The index of the parameter operand.</param>
        public static void EmitLdarga(this ICILWriter writer, int argumentIndex)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (argumentIndex < 0) throw new ArgumentOutOfRangeException("argumentIndex", "argumentIndex may not be negative");

            if (argumentIndex <= byte.MaxValue)
            {
                writer.Emit(OpCodes.Ldarga_S, (byte)argumentIndex);
                return;
            }
            writer.Emit(OpCodes.Ldarga, argumentIndex);
        }

        /// <summary>
        /// 发送<see langword="OpCodes.Ldarg"/>指令，或者根据操作数，自动发射最佳的替代指令，例如OpCodes.Ldarg_3或pCodes.Ldarg_S
        /// </summary>
        public static void EmitLdarg(this ICILWriter writer, int argumentIndex)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (argumentIndex < 0) throw new ArgumentOutOfRangeException("argumentIndex", "argumentIndex may not be negative");

            switch (argumentIndex)
            {
                case 0:
                    writer.Emit(OpCodes.Ldarg_0);
                    return;
                case 1:
                    writer.Emit(OpCodes.Ldarg_1);
                    return;
                case 2:
                    writer.Emit(OpCodes.Ldarg_2);
                    return;
                case 3:
                    writer.Emit(OpCodes.Ldarg_3);
                    return;
                default:
                    if (argumentIndex <= byte.MaxValue)
                    {
                        writer.Emit(OpCodes.Ldarg_S, (byte)argumentIndex);
                        return;
                    }
                    writer.Emit(OpCodes.Ldarg, argumentIndex);
                    return;
            }
        }

        /// <summary>
        /// <para>将位于特定索引处的局部变量加载到计算堆栈上。注意：加载的是值</para>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="localIndex">需要加载的局部变量的索引</param>
        public static void EmitLdloc(this ICILWriter writer, int localIndex)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (localIndex < 0) throw new ArgumentOutOfRangeException("localIndex", "localIndex may not be negative");

            switch (localIndex)
            {
                case 0:
                    writer.Emit(OpCodes.Ldloc_0);
                    return;
                case 1:
                    writer.Emit(OpCodes.Ldloc_1);
                    return;
                case 2:
                    writer.Emit(OpCodes.Ldloc_2);
                    return;
                case 3:
                    writer.Emit(OpCodes.Ldloc_3);
                    return;
                default:
                    if (localIndex <= byte.MaxValue)
                    {
                        //高效加载0-255索引处的变量
                        writer.Emit(OpCodes.Ldloc_S, (byte)localIndex);
                        return;
                    }
                    writer.Emit(OpCodes.Ldloc, localIndex);
                    return;
            }
        }

        /// <summary>
        /// <para>将位于特定索引处的局部变量的地址加载到计算堆栈上。注意：加载的是地址</para>
        /// <para>根据localIndex发送<see langword="OpCodes.Ldloca"/>或 <see langword="OpCodes.Ldloca_S"/>指令</para>
        /// <para><see langword="OpCodes.Ldloca_S"/>指令可以高效的加载索引0-255的局部变量的地址</para>
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="localIndex">需要加载的局部变量的索引</param>
        public static void EmitLdloca(this ICILWriter writer, int localIndex)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (localIndex < 0) throw new ArgumentOutOfRangeException("localIndex", "索引不是能负数");

            if (localIndex <= byte.MaxValue)
            {
                writer.Emit(OpCodes.Ldloca_S, (byte)localIndex);
                return;
            }
            writer.Emit(OpCodes.Ldloca, localIndex);
        }

        /// <summary>
        /// Emits <see langword="OpCodes.Stloc"/> or the best alternative macro depending on the operand.
        /// </summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="localIndex">The index of the local variable operand.</param>
        public static void EmitStloc(this ICILWriter writer, int localIndex)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (localIndex < 0) throw new ArgumentOutOfRangeException("localIndex", "localIndex may not be negative");

            switch (localIndex)
            {
                case 0:
                    writer.Emit(OpCodes.Stloc_0);
                    return;
                case 1:
                    writer.Emit(OpCodes.Stloc_1);
                    return;
                case 2:
                    writer.Emit(OpCodes.Stloc_2);
                    return;
                case 3:
                    writer.Emit(OpCodes.Stloc_3);
                    return;
                default:
                    if (localIndex <= byte.MaxValue)
                    {
                        writer.Emit(OpCodes.Stloc_S, (byte)localIndex);
                        return;
                    }
                    writer.Emit(OpCodes.Stloc, localIndex);
                    return;
            }
        }

        /// <summary>
        /// <para>封装了加载 int 类型数据到计算堆栈的方法</para>
        /// <para>使用该方法，可以不考虑value范围而快捷方便的加载int类型数据到计算堆栈</para>
        /// </summary>
        /// <param name="writer">MSIL写入器</param>
        /// <param name="value">将类型为<see cref="Int32"/>的值加载到计算堆栈上.</param>
        /// <exception cref="ArgumentNullException">
        ///	<para><paramref name="writer"/> 是 <see langword="null"/>.</para>
        /// </exception>
        public static void EmitLdcI4(this ICILWriter writer, int value)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            OpCode opCode;

            switch (value)
            {
                case -1: opCode = OpCodes.Ldc_I4_M1; break;
                case 0: opCode = OpCodes.Ldc_I4_0; break;
                case 1: opCode = OpCodes.Ldc_I4_1; break;
                case 2: opCode = OpCodes.Ldc_I4_2; break;
                case 3: opCode = OpCodes.Ldc_I4_3; break;
                case 4: opCode = OpCodes.Ldc_I4_4; break;
                case 5: opCode = OpCodes.Ldc_I4_5; break;
                case 6: opCode = OpCodes.Ldc_I4_6; break;
                case 7: opCode = OpCodes.Ldc_I4_7; break;
                case 8: opCode = OpCodes.Ldc_I4_8; break;
                default:
                    opCode = OpCodes.Ldc_I4;
                        //value <= byte.MaxValue 如果这样写会有BUG
                        //? OpCodes.Ldc_I4_S
                        //: OpCodes.Ldc_I4;
                    break;
            }

            switch (opCode.OperandType)
            {
                case OperandType.InlineNone:
                    writer.Emit(opCode);
                    break;
                case OperandType.ShortInlineI:
                    writer.Emit(opCode, (byte)value);
                    break;
                case OperandType.InlineI:
                    writer.Emit(opCode, value);
                    break;
                default:
                    throw new InvalidOperationException("意外的 OperandType. 这表示有一个BUG，请修复.");
            }
        }

        public static void EmitCall(this ICILWriter writer, MethodInfo method)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (method == null) throw new ArgumentNullException("method");

            OpCode opCode = !method.IsStatic && method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call;

            writer.Emit(opCode, method);
        }

        /// <summary>
        /// 获取基元类型占用的空间大小
        /// </summary>
        /// <param name="type">基元类型</param>
        /// <returns>基元类型占用的空间大小</returns>
        public static int GetPrimitiveSize(this Type type)
        {
            ArgumentAssert.IsNotNull(type, "type");
            if (!type.IsPrimitive) throw new ArgumentException("type 必须是基元类型", "type");

            return _primitiveSizes[type];
        }

        /// <summary>
        /// 确定的类型是否为浮点型（double或float）。
        /// </summary>
        /// <param name="type">指定的类型</param>
        public static bool IsFloatingPoint(this Type type)
        {
            return type == typeof(float) || type == typeof(double);
        }
    }
}
