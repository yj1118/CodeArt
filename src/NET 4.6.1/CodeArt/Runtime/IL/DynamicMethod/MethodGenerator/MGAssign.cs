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
        #region 赋值操作

        public void AssignVariable(string variableName, Action loadValue)
        {
            IVariable variable = FindVariable(variableName);
            Assign(variable, loadValue);
        }

        /// <summary>
        /// 赋值给变量
        /// </summary>
        /// <param name="variable">需要赋值的变量</param>
        /// <param name="loadValue">加载值的操作</param>
        public void Assign(IVariable variable, Action loadValue)
        {
            BeginAssign(variable);
            loadValue();
            EndAssign();
        }

        /// <summary>
        /// 赋值给变量成员，请确保需要赋值的对象已被加载到堆栈上
        /// </summary>
        /// <param name="memberName"></param>
        /// <param name="loadValue"></param>
        public void Assign(string memberName, Action loadValue)
        {
            BeginAssign(memberName);
            loadValue();
            EndAssign();
        }

        /// <summary>
        /// 赋值给变量字段，请确保需要赋值的对象已被加载到堆栈上
        /// </summary>
        /// <param name="field"></param>
        /// <param name="loadValue"></param>
        public void Assign(FieldInfo field, Action loadValue)
        {
            BeginAssign(field);
            loadValue();
            EndAssign();
        }

        /// <summary>
        /// 赋值给变量属性，请确保需要赋值的对象已被加载到堆栈上
        /// </summary>
        /// <param name="property"></param>
        /// <param name="loadValue"></param>
        public void Assign(PropertyInfo property, Action loadValue)
        {
            BeginAssign(property);
            loadValue();
            EndAssign();
        }

        #region 私有

        private void BeginAssign(IVariable variable)
        {
            ArgumentAssert.IsNotNull(variable, "variable");
            variable.BeginAssign();
            _delayedInstructions.Push(variable.EndAssign);
        }

        /// <summary>
        /// 分配结束
        /// </summary>
        private void EndAssign()
        {
            _delayedInstructions.Pop()();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberName"></param>
        private void BeginAssign(string memberName)
        {
            ArgumentAssert.IsNotNullOrEmpty(memberName, "memberName");

            var target = _evalStack.Peek();
            if (target.ItemType != ItemType.Reference)
                throw new InvalidOperationException("此操作执行之前，请加载引用类型对象到计算堆栈上");

            if (target.Type == null)
                throw new InvalidOperationException("此操作执行之前，请加确保对象已加载到计算堆栈上");

            BeginAssign(GetFieldOrProperty(target.Type, memberName));
        }

        private void BeginAssign(MemberInfo fieldOrProperty)
        {
            if (fieldOrProperty == null) throw new ArgumentNullException("fieldOrProperty");

            if (fieldOrProperty is FieldInfo)
            {
                BeginAssign((FieldInfo)fieldOrProperty);
                return;
            }

            if (fieldOrProperty is PropertyInfo)
            {
                BeginAssign((PropertyInfo)fieldOrProperty);
                return;
            }

            throw new ArgumentException("fieldOrProperty is not an IFieldReference or an IPropertyReference", "fieldOrProperty");
        }


        private void BeginAssign(FieldInfo field)
        {
            if (field == null) throw new ArgumentNullException("field");

            StackItem target = default(StackItem);
            if (!field.IsStatic)
            {
                target = _evalStack.Peek();
                StackAssert.IsAssignable(target, new StackItem(field.DeclaringType, LoadOptions.ValueAsAddress), false);
            }

            _delayedInstructions.Push(() =>
            {
                _evalStack.Pop();
                // 稍后做验证

                if (field.IsStatic)
                {
                    _writer.Emit(OpCodes.Stsfld, field);
                }
                else
                {
                    var item = _evalStack.Pop();
                    if (target != item)
                    {
                        throw new InvalidOperationException("BeginAssign/EndAssign 不匹配.");
                    }
                    _writer.Emit(OpCodes.Stfld, field);
                }
            });
        }


        private void BeginAssign(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException("property");

            var method = property.GetSetMethod(true);

            if (method == null)
            {
                throw new InvalidOperationException(string.Format("属性是只读的，无法赋值.", property.DeclaringType, property.Name));
            }

            BeginCall(method);
            _delayedInstructions.Push(EndCall);
        }

        #endregion

        #endregion
    }
}
