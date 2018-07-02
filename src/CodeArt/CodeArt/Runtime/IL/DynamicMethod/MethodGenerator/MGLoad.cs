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
        /// <summary>
        /// <para>加载type类型的默认值到计算堆栈上</para>
        /// <para>如果type是引用类型，则默认值是null</para>
        /// </summary>
        /// <param name="type"></param>
        public void LoadDefaultOf(Type type)
        {
            // 待办事项 - 加载不同的基元类型
            if (type.IsGenericParameter || type.IsValueType)
            {
                PushScope(new Scope(this, 1));
                {
                    var temp = Declare(type); //声明局部变量
                    LoadVariable(temp, LoadOptions.AnyAsAddress);//加载局部变量到计算堆栈
                    _evalStack.Pop();
                    _writer.Emit(OpCodes.Initobj, type); //初始化并弹出对象
                    Load(temp);//重新加载temp到计算堆栈
                }
                PopScope(null, null);
            }
            else
            {
                LoadNull();
            }
        }

        /// <summary>
        /// 将空引用（O 类型）推送到计算堆栈上。
        /// </summary>
        /// <returns></returns>
        public void LoadNull()
        {
            _writer.Emit(OpCodes.Ldnull);//执行实际IL指令
            _evalStack.Push(typeof(int));//同步自身堆栈数据
        }

        /// <summary>
        /// <para>将int类型推加载到计算堆栈上。</para>
        /// <para>使用该方法，可以不考虑value范围而快捷方便的加载int类型数据到计算堆栈</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Load(int value)
        {
            _writer.EmitLdcI4(value);       //执行实际IL指令
            _evalStack.Push(typeof(int));   //同步自身堆栈数据
        }

        public void Load(bool value)
        {
            Load(value ? 1 : 0);
        }

        public void Load(string value)
        {
            if (value == null)
            {
                LoadNull();
                return;
            }
            if (value.Length == 0)
            {
                LoadField(typeof(string).ResolveField("Empty"));
                return;
            }

            _writer.Emit(OpCodes.Ldstr, value);
            _evalStack.Push(typeof(string));
        }

        /// <summary>
        /// 将变量的值加载到堆栈
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public void Load(IVariable variable)
        {
            LoadVariable(variable, LoadOptions.Default);
        }

        /// <summary>
        /// <para>以options的方法加载变量到堆栈</para>
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <remarks>
        /// 在variable.Load(options)之后
        /// 这里并没有调用_evalStack.Push
        /// 是因为有个假设：IVariable 是<see cref="LocalDefinition"/>类型
        /// 在该类型里的Load方法中，执行了_evalStack.Push
        /// 由此引起的一个问题是，Load方法是公开的，如果消费者创建自己的IVariable，用Load方法加载
        /// 就会引起BUG,但是消费者很少会自己创建IVariable，都交由<see cref="MethodGenerator"/>自己负责
        /// 可以将 public 改为 Private 可以解决
        /// 这里先保留public
        /// </remarks>
        public void LoadVariable(IVariable variable, LoadOptions options)
        {
            variable.Load(options);
        }

        private IVariable FindVariable(string name)
        {
            foreach (var localDef in _locals)
            {
                if (localDef.InScope
                    && localDef.Name == name)
                {
                    return new BorrowedLocal(localDef);
                }
            }
            throw new ApplicationException("没有找到名称为" + name + "的局部变量");
        }

        public void LoadVariable(string name)
        {
            LoadVariable(FindVariable(name), LoadOptions.Default);
        }

        public void LoadVariable(string name, LoadOptions options)
        {
            LoadVariable(FindVariable(name), options);
        }

        public void Load(string name, LoadOptions options)
        {
            LoadVariable(FindVariable(name), options);
        }

        #region 加载成员

        public void LoadMember(string memberName)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                throw new ArgumentNullException("memberName");
            }

            var item = _evalStack.Peek();

            // todo - fix to work with generics
            //if (item.Type.IsValueType)
            //{
            //   if (item.ItemType != EvalStackItemType.Address)
            //   {
            //      throw new InvalidOperationException("The last item on the stack must be a " + EvalStackItemType.Address);
            //   }
            //}
            //else
            //{
            //   if (item.ItemType != EvalStackItemType.Reference)
            //   {
            //      throw new InvalidOperationException("The last item on the stack must be a " + EvalStackItemType.Reference);
            //   }
            //}

            //if (item.Type == null)
            //{
            //   throw new InvalidOperationException("The last item on the stack is null or doesn't have a type.");
            //}

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var field = item.Type.GetField(memberName, flags);
            if (field != null)
            {
                LoadField(field);
                return;
            }

            var property = item.Type.ResolveProperty(memberName);
            if (property != null)
            {
                LoadProperty(property);
                return;
            }

            throw new InvalidOperationException(string.Format("{0} 没有定义字段或属性 {1}", item.Type.Name, memberName));
        }

        public void LoadField(FieldInfo field)
        {
            LoadField(field, LoadOptions.Default);
        }

        public void LoadField(FieldInfo field, LoadOptions mode)
        {
            if (field == null) throw new ArgumentNullException("field");

            OpCode opCode;

            if (ShouldLoadAddress(field.FieldType, mode))
            {
                opCode = field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda;
            }
            else
            {
                opCode = field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld;
            }

            if (!field.IsStatic)
            {
                StackAssert.IsAssignable(_evalStack.Pop(), new StackItem(field.DeclaringType, LoadOptions.ValueAsAddress), false);
            }
            _writer.Emit(opCode, field);
            _evalStack.Push(field.FieldType, mode);
            return;
        }

        public void LoadProperty(PropertyInfo property)
        {
            LoadProperty(property, LoadOptions.Default);
        }

        public void LoadProperty(PropertyInfo property, LoadOptions options)
        {
            if (property == null) throw new ArgumentNullException("property");

            if (options.ShouldLoadAddress(property.PropertyType))
            {
                using (var tempLocal = BorrowLocal(property.PropertyType, false, null))
                {
                    BeginAssign(tempLocal);
                    Call(property.GetGetMethod(true));
                    EndAssign();
                    LoadVariable(tempLocal, LoadOptions.AnyAsAddress);
                }
            }
            else
            {
                Call(property.GetGetMethod(true));
            }
        }


        #endregion


        #region 加载参数

        /// <summary>
        /// 将索引index处的参数加载到堆栈上
        /// </summary>
        /// <param name="index"></param>
        public void LoadParameter(int index)
        {
            var p = this.GetParameter(index);
            this.Load(p);
        }

        #endregion


    }
}
