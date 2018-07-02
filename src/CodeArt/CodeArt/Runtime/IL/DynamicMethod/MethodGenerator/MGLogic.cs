using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace CodeArt.Runtime.IL
{
    [DebuggerDisplay("ScopeDepth={_scopeStack.Count}, EvalStackDepth={_evalStack.Count}")]
    public partial class MethodGenerator
    {
        #region 逻辑判断操作

        public void And()
        {
            StackAssert.IsAssignable(_evalStack.Pop(), new StackItem(typeof(bool)), false);
            StackAssert.IsAssignable(_evalStack.Pop(), new StackItem(typeof(bool)), false);
            _writer.Emit(OpCodes.And);
            _evalStack.Push(typeof(bool));
        }

        public void And(Func<LogicOperator> condition0, Func<LogicOperator> condition1)
        {
            if (condition0 == null) throw new ArgumentNullException("condition0");
            if (condition1 == null) throw new ArgumentNullException("condition1");

            this.Compare(condition0());
            this.Compare(condition1());
            this.And();
        }

        public void Or()
        {
            StackAssert.IsAssignable(_evalStack.Pop(), new StackItem(typeof(bool)), false);
            StackAssert.IsAssignable(_evalStack.Pop(), new StackItem(typeof(bool)), false);
            _writer.Emit(OpCodes.Or);
            _evalStack.Push(typeof(bool));
        }

        public void Or(Func<LogicOperator> condition0, Func<LogicOperator> condition1)
        {
            if (condition0 == null) throw new ArgumentNullException("condition0");
            if (condition1 == null) throw new ArgumentNullException("condition1");

            this.Compare(condition0());
            this.Compare(condition1());
            this.Or();
        }

        /// <summary>
        /// 比较操作
        /// </summary>
        /// <param name="op"></param>
        public void Compare(LogicOperator op)
        {
            for (int i = 0; i < op.ArgumentCount; ++i)
            {
                // todo validate better
                _evalStack.Pop();
            }
            op.WriteCompare(_writer);
            _evalStack.Push(typeof(bool));
        }
        
        #endregion
    }
}
