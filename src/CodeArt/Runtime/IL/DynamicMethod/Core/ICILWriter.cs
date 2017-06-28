using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 	<para>封装了CIL发射指令</para>
    /// </summary>
    public interface ICILWriter : IDisposable
    {
        /// <summary>
        /// 获取方法基本信息
        /// </summary>
        MethodHeader MethodHeader { get; }

        /// <summary>
        /// 根据指定的索引得到类型为<see cref="IVariable"/>的参数
        /// </summary>
        /// <param name="index">需要获取的参数的索引</param>
        /// <returns></returns>
        IVariable GetParameter(int index);

        /// <summary>
        /// 定义一个局部变量，并返回IVariable表示它
        /// </summary>
        /// <param name="type">局部变量的类型</param>
        /// <param name="isPinned">变量是否是固定的</param>
        /// <param name="name">局部变量的名称，如果为null表示不指定</param>
        /// <returns>
        ///	<para>An <see cref="IVariable"/> representation of the defined local.</para>
        /// </returns>
        IVariable DefineLocal(Type type, bool isPinned, string name);

        /// <summary>
        /// 定义一个可以被引用和标记的标签
        /// </summary>
        /// <returns>
        ///	<para>一个 <see cref="ILabel"/> 可以被引用和标记的标签实例</para>
        /// </returns>
        ILabel DefineLabel();

        /// <summary>
        /// 开始try块；在块中的指令抛出异常，会由try块截获
        /// </summary>
        void BeginTry();


        /// <summary>
        /// 开始catch块；在块中的指令抛出异常，会由catch块处理
        /// </summary>
        /// <param name="exceptionType"></param>
        void BeginCatch(Type exceptionType);

        
        /// <summary>
        /// 开始finally块
        /// </summary>
        void BeginFinally();

        void EndTryCatchFinally();

        /// <summary>
        /// 将指定的指令放入指令流上
        /// </summary>
        /// <param name="opCode"></param>
        void Emit(OpCode opCode);

        void Emit(OpCode opCode, Type operand);

        void Emit(OpCode opCode, MethodInfo operand);

        void Emit(OpCode opCode, FieldInfo operand);

        void Emit(OpCode opCode, ConstructorInfo operand);

        /// <summary>
        /// 将指定的跳转指令放入指令流上
        /// </summary>
        /// <param name="opCode"></param>
        /// <param name="operand"></param>
        void Emit(OpCode opCode, ILabel operand);

        void Emit(OpCode opCode, string operand);

        void Emit(OpCode opCode, int operand);

        void Emit(OpCode opCode, long operand);

        void Emit(OpCode opCode, byte operand);

        void Emit(OpCode opCode, float operand);

        void Emit(OpCode opCode, double operand);
    }
}
