using System;
using System.Diagnostics;
using System.Reflection.Emit;




namespace CodeArt.Runtime.IL
{
    [DebuggerDisplay("ScopeDepth={_scopeStack.Count}, EvalStackDepth={_evalStack.Count}")]
    public partial class MethodGenerator
    {
        #region while

        public void While(Func<LogicOperator> condition, Action loop)
        {
            var startLabel = _writer.DefineLabel();
            var conditionLabel = _writer.DefineLabel();

            PushScope(new Scope(this));

            _writer.Emit(OpCodes.Br, conditionLabel);//将控制转移到conditionLabel标签，该标签稍后会标记MSIL流位置，因此，在执行的时候，会先执行conditionLabel指向的指令(注释1)
            startLabel.Mark();//标记MSIL流中循环体位置
            loop();
            conditionLabel.Mark();//标记MSIL流中条件位置,因为已经通过(注释1)处的代码将控制转移到conditionLabel标签了，而此处又标记了流位置
            //因此，在实际执行代码时，会先执行conditionLabel的代码，条件判断后，才会执行startLabel的代码
            var c = condition();
            GoToIf(c, startLabel);

            PopScope(typeof(Scope), "循环代码段已经打开，但是不能关闭，有可能是循环体或者条件体内有代码段闭合错误");
        }

        #endregion

        #region foreach

        public void ForEach(Type elementType, Action<IVariable> action)
        {
            var element = BeginForEach(elementType);
            action(element);
            EndForEach();
        }

        public void ForEach(Action<IVariable> action)
        {
            var item = _evalStack.Peek();
            ForEach(item.Type.ResolveElementType(), action);
        }

        private IVariable BeginForEach(Type elementType)
        {
            var scope = new ForEachScope(this, elementType);
            PushScope(scope);
            return scope.Current;
        }

        private void EndForEach()
        {
            PopScope(typeof(ForEachScope), "BeginForEach/EndForEach mismatch");
        }

        #endregion

        #region for

        public void For(IVariable count, Action<IVariable> action)
        {
            For(0, count, action);
        }


        public void For(int startIndex, IVariable count,Action<IVariable> action)
        {
            this.BeginScope();

            IVariable i = this.Declare<int>();
            this.Assign(i, () =>
            {
                this.Load(startIndex);
            });

            this.While(() =>
               {
                   this.Load(i);
                   this.Load(count);
                   return LogicOperator.LessThan;
               },
               () =>
               {
                   action(i);

                   this.Increment(i);
               });

            this.EndScope();
        }

        #endregion


        #region 存储项

        public void StoreElement(IVariable array, IVariable i,IVariable element)
        {
            this.Load(array);
            this.BeginStoreElement();
            this.Load(i);
            this.Load(element);
            this.EndStoreElement();
        }

        /// <summary>
        /// <para>开始将值存入数组指定的索引处</para>
        /// <para>示例代码：</para>
        /// <code> 
        /// g.Load(context.Member);
        /// g.BeginStoreElement();
        ///	{
        ///		g.Load(i);
        ///		g.Load(element);
        ///	}
        ///	g.EndStoreElement();
        /// </code>
        /// </summary>
        /// <remarks>
        /// 注意：由于BeginStoreElement前后，分为三步：
        /// 1.将对数组 array 的对象引用推送到堆栈上。
        /// 2.将 array 中元素的索引值 index 推送到堆栈上。
        /// 3.将指令中指定类型的值推送到堆栈上。
        /// 因此，以下代码，要反向验证，即：堆栈顶部的数据为：
        /// 1.需要存储的值
        /// 2.索引值 index 
        /// 3.数组 array 的对象引用
        /// </remarks>
        private void BeginStoreElement()
        {
            _delayedInstructions.Push(() =>
            {
                //需要存储的值
                var valueItem = _evalStack.Pop();

                //验证索引值为int型
                StackAssert.IsAssignable(_evalStack.Pop(), typeof(int), false);

                //将对数组 array 的对象引用推送到堆栈上。
                var arrayItem = _evalStack.Pop();
                //检查是否为array类型
                StackAssert.IsAssignable(arrayItem, typeof(Array), false);

                Debug.Assert(arrayItem.Type.IsArray, "项必须存储在数组类型上");

                var elementType = arrayItem.Type.GetElementType();

                StackAssert.IsAssignable(valueItem, new StackItem(elementType, LoadOptions.Default), true);

                _writer.EmitStelem(elementType);
            });
        }

        private void EndStoreElement()
        {
            _delayedInstructions.Pop()();
        }

        #endregion


        #region 得到项

        public void LoadElement(IVariable array, IVariable i)
        {
            this.Load(array);
            this.BeginLoadElement();
            this.Load(i);
            this.EndLoadElement();
        }

        /// <summary>
        /// <para>开始加载数组元素</para>
        /// <para>请在执行该方法之后，加载索引：</para>
        /// <code> 
        /// g.BeginLoadElement();
        ///	{
        ///		g.Load(i);
        ///	}
        /// g.EndLoadElement();
        /// </code>
        /// </summary>
        private void BeginLoadElement()
        {
            _delayedInstructions.Push(() =>
            {
                //需要加载的成员的索引值
                var item = _evalStack.Pop();
                StackAssert.IsAssignable(item, new StackItem(typeof(int)), false);//检查索引值是否为int型
                //目标数组
                //此处是模拟弹出，用于效验
                item = _evalStack.Pop();
                StackAssert.IsAssignable(item, new StackItem(typeof(Array)), false);//检查弹出的项是否为array

                //var elementType = item.Type.GetElementType();
                var elementType = item.Type.ResolveElementType();
                _writer.EmitLdelem(elementType);//将指定数组索引中的元素加载到计算堆栈的顶部。
                _evalStack.Push(elementType);
            });
        }

        /// <summary>
        /// <para>结束加载</para>
        /// <para>此时会真正执行指令</para>
        /// </summary>
        private void EndLoadElement()
        {
            _delayedInstructions.Pop()();
        }

        #endregion

    }
}
