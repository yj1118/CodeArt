using System.Diagnostics;
using System.Reflection.Emit;


namespace CodeArt.Runtime.IL
{
    [DebuggerDisplay("ScopeDepth={_scopeStack.Count}, EvalStackDepth={_evalStack.Count}")]
    public partial class MethodGenerator
    {
        #region 自增或自检操作

        private void Increment()
        {
            var item = _evalStack.Pop();
            StackAssert.IsAssignable(item, typeof(int), false);
            _writer.EmitLdcI4(1);
            _writer.Emit(OpCodes.Add);
            _evalStack.Push(typeof(int));
        }

        /// <summary>
        /// 变量自增
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public void Increment(IVariable variable)
        {
            this.Load(variable);
            this.Increment();
            this.Store(variable);
        }

        private void Decrement()
        {
            var item = _evalStack.Pop();
            StackAssert.IsAssignable(item, typeof(int), false);
            _writer.EmitLdcI4(1);
            _writer.Emit(OpCodes.Sub);
            _evalStack.Push(typeof(int));
        }

        /// <summary>
        /// 变量自减
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public void Decrement(IVariable variable)
        {
            this.Load(variable);
            this.Decrement();
            this.Store(variable);
        }

        #endregion

        #region 加法

        public void Add<T>()
        {
            StackAssert.IsAssignable(_evalStack.Pop(), typeof(T), false);
            StackAssert.IsAssignable(_evalStack.Pop(), typeof(T), false);
            _writer.Emit(OpCodes.Add);
            _evalStack.Push(typeof(T));
        }

        public void Sub<T>()
        {
            StackAssert.IsAssignable(_evalStack.Pop(), typeof(T), false);
            StackAssert.IsAssignable(_evalStack.Pop(), typeof(T), false);
            _writer.Emit(OpCodes.Sub);
            _evalStack.Push(typeof(T));
        }

        #endregion



    }
}
