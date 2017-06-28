using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using CodeArt.Util;

namespace CodeArt.Runtime.IL
{
    [DebuggerDisplay("ScopeDepth={_scopeStack.Count}, EvalStackDepth={_evalStack.Count}")]
    public partial class MethodGenerator
    {
        #region 构造对象

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="loadArgs"></param>
        public void NewObject(ConstructorInfo constructor, Action loadArgs)
        {
            this.BeginNewObject(constructor);
            loadArgs();
            this.EndNewObject();
        }

        public void NewObject(ConstructorInfo constructor)
        {
            this.BeginNewObject(constructor);
            this.EndNewObject();
        }


        private void BeginNewObject(ConstructorInfo constructor)
        {
            if (constructor == null) throw new ArgumentNullException("constructor");
            if (constructor.IsStatic) throw new ArgumentException("constructor 必须是一个实例构造方法", "constructor");
            if (constructor.DeclaringType.IsArray) throw new ArgumentException("声明的类型是array，请用NewArray方法", "constructor");

            _delayedInstructions.Push(() =>
            {
                PopAndValidateParams(constructor);
                _writer.Emit(OpCodes.Newobj, constructor);
                //将对新对象的引用推送到模拟计算堆栈上。
                _evalStack.Push(constructor.DeclaringType);
            });
        }

        private void EndNewObject()
        {
            _delayedInstructions.Pop()();
        }


        /// <summary>
        /// 构造新对象，调用该方法之前，请确保已向计算堆栈中加载了需要的参数
        /// </summary>
        /// <param name="declaringType"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public void NewObject(Type declaringType, params Type[] parameterTypes)
        {
            if (declaringType == null) throw new ArgumentNullException("declaringType");
            if (declaringType.IsArray) throw new ArgumentException("声明的类型是array，请用NewArray方法", "declaringType");

            parameterTypes = parameterTypes ?? Type.EmptyTypes;

            //var ctor = declaringType.GetConstructor(
            //    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            //    null,
            //    parameterTypes,
            //    null);

            var ctor = declaringType.ResolveConstructor(parameterTypes);

            if (ctor == null)
            {
                var parameters = string.Join(", ", parameterTypes.Select<Type, string>(t => t.Name).ToArray<string>());
                throw new MissingMemberException(declaringType.Name, ".ctor(" + parameters + ")");
            }
            NewObject(ctor);
        }

        public void NewObject(Type declaringType)
        {
            NewObject(declaringType, null);
        }

        #endregion


        #region array

        public void NewArray(Type elementType, Action loadLength)
        {
            NewArray(elementType, 1, loadLength);
        }


        public void NewArray(Type elementType,int rank, Action loadLength)
        {
            this.BeginNewArray(elementType, rank);
            loadLength();
            this.EndNewArray();
        }

        /// <summary>
        /// <para>开始创建新的数组</para>
        /// <para>请在此方法时候，调用加载数组长度的方法，例如：</para>
        /// <code>
        /// g.BeginNewArray(_elementType);
        ///	{
        ///		g.Load(count);
        ///	}
        ///	g.EndNewArray();
        /// </code>
        /// </summary>
        /// <param name="elementType">数组的成员类型</param>
        /// <param name="rank">维数</param>
        private void BeginNewArray(Type elementType, int rank)
        {
            if (elementType == null) throw new ArgumentNullException("elementType");
            if (rank < 1) throw new ArgumentOutOfRangeException("rank", "数组维度不能小于1");

            var arrayType = rank == 1 ? elementType.MakeArrayType() : elementType.MakeArrayType(rank);
            _delayedInstructions.Push(() =>
            {
                for (int i = 0; i < rank; ++i)
                {
                    StackAssert.AreEqual(_evalStack.Pop(), typeof(int));
                }

                if (rank == 1)
                {
                    //创建一维数组
                    //将对新的从零开始的一维数组（其元素属于特定类型）的对象引用推送到计算堆栈上
                    _writer.Emit(OpCodes.Newarr, elementType);
                }
                else
                {
                    var paramTypes = new Type[rank];
                    for (int i = 0; i < paramTypes.Length; ++i)
                    {
                        paramTypes[i] = typeof(int);
                    }
                    //创建多维数组
                    //利用多维数组的构造函数，创建多维数组对象
                    _writer.Emit(OpCodes.Newobj, arrayType.GetConstructor(paramTypes));
                }
                _evalStack.Push(arrayType);
            });
        }

        /// <summary>
        /// 创建数组结束
        /// </summary>
        private void EndNewArray()
        {
            _delayedInstructions.Pop()();
        }

        #endregion
    }
}
