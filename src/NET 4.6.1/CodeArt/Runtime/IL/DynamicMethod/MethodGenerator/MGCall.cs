using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;


namespace CodeArt.Runtime.IL
{
    [DebuggerDisplay("ScopeDepth={_scopeStack.Count}, EvalStackDepth={_evalStack.Count}")]
    public partial class MethodGenerator
    {
        #region 调用方法的操作


        public void Call(MethodInfo method,Action loadParameters)
        {
            BeginCall(method);
            loadParameters();
            EndCall();
        }

        private void BeginCall(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException("method");
            _delayedInstructions.Push(() => Call(method));
        }

        /// <summary>
        /// Ends a method call.
        /// </summary>
        private void EndCall()
        {
            _delayedInstructions.Pop()();
        }

        public void Call(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException("method");

            StackItem lastItem = default(StackItem);
            bool arrayCheck = !method.IsStatic && method.Name == "get_Length" && _evalStack.Count > 0;
            if (arrayCheck)
            {
                lastItem = _evalStack.Peek();
            }

            PopAndValidateParams(method);

            if (arrayCheck
                && lastItem.Type.IsArray
                && lastItem.Type.GetArrayRank() == 1)
            {
                _writer.Emit(OpCodes.Ldlen);
                _writer.Emit(OpCodes.Conv_I4);
            }
            else
            {
                _writer.EmitCall(method);
            }
            if (method.ReturnParameter != null && typeof(void) != method.ReturnType)
            {
                _evalStack.Push(method.ReturnType);
            }
        }

        #endregion

    }
}
