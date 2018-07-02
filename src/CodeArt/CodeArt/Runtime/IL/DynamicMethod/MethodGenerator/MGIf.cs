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
        #region if 判断
        public void If(Func<LogicOperator> condition, Action trueAction)
        {
            this.If(condition);
            trueAction();
            this.EndIf();
        }

        public void If(Func<LogicOperator> condition, Action trueAction,Action falseAction)
        {
            this.If(condition);
            trueAction();
            this.Else();
            falseAction();
            this.EndIf();
        }

        public void If(Func<LogicOperator> condition)
        {
            if (condition == null) throw new ArgumentNullException("condition");

            var c = condition();
            var ifScope = new LabeledScope(this);
            GoToIf(c.Negate(), ifScope.EndLabel);
            PushScope(ifScope);
        }

        private void Else()
        {
            var elseScope = new LabeledScope(this);
            _writer.Emit(OpCodes.Br, elseScope.EndLabel);
            PopScope(typeof(LabeledScope), "Else 没有对应的 If");
            PushScope(elseScope);
        }

        public void EndIf()
        {
            PopScope(typeof(LabeledScope), "EndIf 没有对应的 If");
        }

        #endregion
    }
}
