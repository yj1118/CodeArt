using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArt.Runtime.IL
{
    /// <summary>
    /// 	<para>一个 <see cref="ICILWriter"/> 的实现，为<see cref="DynamicMethod"/>实例写入IL流</para>
    /// </summary>
    public class MSILWriter : ICILWriter
    {
        private readonly SimpleParameter[] _parameters;
        private readonly ILGenerator _ilGenerator;
        private readonly MethodHeader _methodHeader;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method">需要写入IL流的<see cref="DynamicMethod"/>实例</param>
        public MSILWriter(DynamicMethod method)
        {
            var parameters = method.GetParameters();
            _methodHeader = new MethodHeader
            {
                Attributes = method.CallingConvention,
                DeclaringType = method.DeclaringType,
                ParameterTypes = method.GetParameters()
                    .Select<ParameterInfo, Type>(p => p.ParameterType)
                    .ToArray<Type>(),
                ReturnType = method.ReturnType ?? typeof(void)
            };
            _parameters = new SimpleParameter[parameters.Length];
            int positionOffset = method.IsStatic ? 0 : 1;
            for (int i = 0; i < parameters.Length; ++i)
            {
                _parameters[i] = new SimpleParameter(
                    this,
                    parameters[i].Position + positionOffset,
                    parameters[i].ParameterType,
                    null);
            }
            _ilGenerator = method.GetILGenerator();
        }

        public MSILWriter(MethodBase method, ILGenerator ilGenerator)
        {
            var parameters = method.GetParameters();
            _methodHeader = new MethodHeader
            {
                Attributes = method.CallingConvention,
                DeclaringType = method.DeclaringType,
                ParameterTypes = method.GetParameters()
                    .Select<ParameterInfo, Type>(p => p.ParameterType)
                    .ToArray<Type>(),
                ReturnType = typeof(void)
            };
            _parameters = new SimpleParameter[parameters.Length];
            int positionOffset = method.IsStatic ? 0 : 1;
            for (int i = 0; i < parameters.Length; ++i)
            {
                _parameters[i] = new SimpleParameter(
                    this,
                    parameters[i].Position + positionOffset,
                    parameters[i].ParameterType,
                    null);
            }
            _ilGenerator = ilGenerator;
        }

        #region ICILWriter 成员

        MethodHeader ICILWriter.MethodHeader
        {
            [DebuggerStepThrough]
            get { return _methodHeader; }
        }

        IVariable ICILWriter.GetParameter(int index)
        {
            return _parameters[index];
        }

        IVariable ICILWriter.DefineLocal(Type type, bool isPinned, string name)
        {
            var localBuilder = _ilGenerator.DeclareLocal(type, isPinned);
            return new SimpleLocal(this, localBuilder.LocalIndex, type, name, localBuilder.IsPinned);
        }

        /// <summary>
        /// 定义标签
        /// </summary>
        /// <returns></returns>
        ILabel ICILWriter.DefineLabel()
        {
            return new LabelInfo(this, _ilGenerator.DefineLabel());
        }

        void ICILWriter.BeginTry()
        {
            _ilGenerator.BeginExceptionBlock();
        }

        void ICILWriter.BeginCatch(Type exceptionType)
        {
            _ilGenerator.BeginCatchBlock(exceptionType);
        }

        void ICILWriter.BeginFinally()
        {
            _ilGenerator.BeginFinallyBlock();
        }

        void ICILWriter.EndTryCatchFinally()
        {
            _ilGenerator.EndExceptionBlock();
        }

        void ICILWriter.Emit(OpCode opCode)
        {
            _ilGenerator.Emit(opCode);
        }

        void ICILWriter.Emit(OpCode opCode, byte operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, int operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, long operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, float operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, double operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, string operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, Type operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, MethodInfo operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, ConstructorInfo operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, FieldInfo operand)
        {
            _ilGenerator.Emit(opCode, operand);
        }

        void ICILWriter.Emit(OpCode opCode, ILabel label)
        {
            var l = (LabelInfo)label; //强制转换
            l.IsReferenced = true;
            _ilGenerator.Emit(opCode, l.Label);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        private class LabelInfo : ILabel
        {
            private readonly MSILWriter _writer;
            /// <summary>
            /// 表示指令流中的标签。Label 与 ILGenerator 类一起使用。
            /// </summary>
            private readonly Label _label;

            public LabelInfo(MSILWriter writer, Label label)
            {
                _writer = writer;
                _label = label;
            }

            public Label Label
            {
                get { return _label; }
            }

            /// <summary>
            /// 将MSIL流的当前位置标记给该标签
            /// </summary>
            public void Mark()
            {
                _writer._ilGenerator.MarkLabel(_label);
                IsMarked = true;
            }

            public bool IsReferenced { get; set; }

            public bool IsMarked { get; set; }
        }
    }
}
