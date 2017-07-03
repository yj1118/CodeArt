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
    /// <summary>
    /// 方法生成器
    /// 封装了功能：用给定的 <see cref="ICILWriter"/>实例写一个方法
    /// 这个类有很好的特点，比如 循环、计算堆栈验证
    /// 通常不会直接对<see cref="ILGenerator"/> 或 <see cref="ICILWriter"/> 编码
    /// </summary>
    [DebuggerDisplay("ScopeDepth={_scopeStack.Count}, EvalStackDepth={_evalStack.Count}")]
    public partial class MethodGenerator
    {
        /// <summary>
        /// 获取对Dispose方法的元数据访问信息
        /// </summary>
        private static readonly MethodInfo _disposeMethod = typeof(IDisposable).GetMethod("Dispose");

        private readonly ICILWriter _writer;
        private readonly MethodHeader _header;
        private Parameter _thisParameter;
        private readonly Func<int, Parameter> _parameters;
        /// <summary>
        /// 局部变量的集合
        /// 该集合存放了所有声明过的局部变量
        /// </summary>
        private readonly List<LocalDefinition> _locals = new List<LocalDefinition>();
        /// <summary>
        /// <para>计模拟算堆栈</para>
        /// <para>可以用于追踪方法执行的过程，例如验证操作等</para>
        /// </summary>
        private readonly EvaluationStack _evalStack = new EvaluationStack();

        /// <summary>
        /// 代码范围的堆栈
        /// 该字段用来解决类似：
        /// <code>
        /// {
        ///     //....
        ///     {
        ///         //.....
        ///     }
        /// }
        /// </code>
        /// 代码段嵌套的问题（比如变量作用域等）
        /// </summary>
        private readonly Stack<Scope> _scopeStack = new Stack<Scope>();
        /// <summary>
        /// <para>延迟执行的指令</para>
        /// <para>在执行创建数组、加载数组元素等操作时，需要先将相关的数据（例如元素索引等）加载到堆栈，再执行操作</para>
        /// <para>用延迟指令机制，可以让消费者在调用时，先执行操作，再加载数据，这样更符合消费者的调用逻辑</para>
        /// </summary>
        private readonly Stack<Action> _delayedInstructions = new Stack<Action>();

        /// <summary>
        /// 代码执行的当前代码段
        /// </summary>
        private Scope _currentScope;

        #region 构造

        /// <summary>
        /// 初始化一个 <see cref="MethodGenerator"/> 类的实例.
        /// </summary>
        /// <param name="writer">指令写入器</param>
        private MethodGenerator(ICILWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            var header = writer.MethodHeader;

            //检查非静态方法是否传入了声明类型
            if (header.DeclaringType == null && NotStaticMethod(header))
            {
                throw new ArgumentException("必须为非静态方法定义一个声明类型（方法所在类的类型）");
            }

            //检查参数是否有null
            for (int i = 0; i < header.ParameterTypes.Length; ++i)
            {
                if (header.ParameterTypes[i] == null) 
                    throw new ArgumentException("writer.Header.Parameters 不能包含null值", "writer");
            }

            _writer = writer;
            _header = header;
            //收集参数
            _parameters = LazyIndexer.Init<int, Parameter>(parameterIndex =>
            {
                if (NotStaticMethod(_header)) ++parameterIndex;//如果不是静态方法，那么将参数索引值+1，因为第0个参数是this(也就是对象本身)
                return new Parameter(this, _writer.GetParameter(parameterIndex));
            });
            _currentScope = new Scope(this); //创建代码范围
        }

        /// <summary>
        /// 创建基于MSIL流的方法
        /// </summary>
        /// <param name="name">方法名称</param>
        /// <param name="returnType">返回值</param>
        /// <param name="parameterTypes">参数</param>
        /// <returns></returns>
        public MethodGenerator(DynamicMethod method)
            : this(new MSILWriter(method))
        {
        }

        public MethodGenerator(MethodBase method, ILGenerator ilGenerator)
            : this(new MSILWriter(method, ilGenerator))
        {
        }

        #endregion

        /// <summary>
        /// 判断不是静态方法
        /// </summary>
        /// <param name="header"></param>
        /// <returns>true:不是静态方法,false:是静态方法</returns>
        private static bool NotStaticMethod(MethodHeader header)
        {
            return (header.Attributes & CallingConventions.HasThis) == CallingConventions.HasThis;
        }

        /// <summary>
        /// ???????????????????????????????
        /// </summary>
        /// <param name="startVariable"></param>
        /// <returns></returns>
        public IExpression CreateExpression(IVariable startVariable)
        {
            if (startVariable == null) throw new ArgumentNullException("startVariable");

            return new Expression(this, startVariable);
        }

        /// <summary>
        /// 对栈顶部的数据进行装箱
        /// </summary>
        /// <returns></returns>
        public MethodGenerator Box()
        {
            var item = _evalStack.Pop();
            if (!item.Type.IsValueType)
                throw new InvalidOperationException(string.Format("计算堆栈上的数据 ({0}) 不是值类型，无法执行装箱操作", item.Type.Name));

            if (item.IsAddress)
            {
                throw new InvalidOperationException("计算堆栈上的数据为一个引用地址.装箱操作需要的是数据值.");
            }
            _writer.Emit(OpCodes.Box, item.Type);
            _evalStack.Push(item.Type, LoadOptions.BoxValues);
            return this;
        }

        #region 类型转换

        public void Convert(Type toType, bool withOverflowCheck)
        {
            var item = _evalStack.Pop();
            StackAssert.IsPrimitive(item);
            OpCode conversionCode;
            if (_conversions.TryGetValue(new ConversionKey(toType, withOverflowCheck), out conversionCode))
            {
                _writer.Emit(conversionCode);
                _evalStack.Push(toType);
                return;
            }

            throw new ArgumentException("不能转换类型 " + toType.Name, "toType");
        }

        private struct ConversionKey
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="to"></param>
            /// <param name="overflowCheck">溢出检查</param>
            public ConversionKey(Type to, bool overflowCheck)
            {
                To = to;
                OverflowCheck = overflowCheck;
            }

            public readonly Type To;
            public readonly bool OverflowCheck;
        }

        private class ConversionKeyEqualityComparer : IEqualityComparer<ConversionKey>
        {
            public bool Equals(ConversionKey x, ConversionKey y)
            {
                return x.OverflowCheck == y.OverflowCheck
                    && x.To == y.To;
            }

            public int GetHashCode(ConversionKey obj)
            {
                //一个类型常具有多个可以参与生成哈希代码的数据字段。
                //生成哈希代码的一种方法是使用 XOR (eXclusive OR) 运算合并这些字段
                //XOR:只有在两个比较的位不同时其结果是1，否则结果为0
                return obj.To.GetHashCode() ^ obj.OverflowCheck.GetHashCode();
            }
        }

        private static readonly Dictionary<ConversionKey, OpCode> _conversions =
            new Dictionary<ConversionKey, OpCode>(new ConversionKeyEqualityComparer())
		{
			// 有符号，不许溢出
			{ new ConversionKey(typeof(sbyte), false), OpCodes.Conv_I1 },
			{ new ConversionKey(typeof(short), false), OpCodes.Conv_I2 },
			{ new ConversionKey(typeof(int), false), OpCodes.Conv_I4 },
			{ new ConversionKey(typeof(long), false), OpCodes.Conv_I8 },
			// 无符号, 不许溢出
			{ new ConversionKey(typeof(byte), false), OpCodes.Conv_I1 },
			{ new ConversionKey(typeof(ushort), false), OpCodes.Conv_I2 },
			{ new ConversionKey(typeof(uint), false), OpCodes.Conv_I4 },
			{ new ConversionKey(typeof(ulong), false), OpCodes.Conv_I8 },
			// 有符号, 允许溢出
			{ new ConversionKey(typeof(sbyte), true), OpCodes.Conv_Ovf_I1 },
			{ new ConversionKey(typeof(short), true), OpCodes.Conv_Ovf_I2 },
			{ new ConversionKey(typeof(int), true), OpCodes.Conv_Ovf_I4 },
			{ new ConversionKey(typeof(long), true), OpCodes.Conv_Ovf_I8 },
			// 无符号, 允许溢出
			{ new ConversionKey(typeof(byte), true), OpCodes.Conv_Ovf_I1_Un },
			{ new ConversionKey(typeof(ushort), true), OpCodes.Conv_Ovf_I2_Un },
			{ new ConversionKey(typeof(uint), true), OpCodes.Conv_Ovf_I4_Un },
			{ new ConversionKey(typeof(ulong), true), OpCodes.Conv_Ovf_I8_Un }
		};

        

        #endregion

        /// <summary>
        /// 移除当前位于计算堆栈顶部的值
        /// </summary>
        public void Pop()
        {
            _evalStack.Pop();
            _writer.Emit(OpCodes.Pop);
        }

        /// <summary>
        /// 抛出异常
        /// </summary>
        public void Throw()
        {
            var item = _evalStack.Pop();
            StackAssert.IsAssignable(item, typeof(Exception), false);
            _writer.Emit(OpCodes.Throw);
        }

        /// <summary>
        /// 三元运算
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        /// <param name="condition"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        public void Ternary(Type type, LoadOptions options, Func<LogicOperator> condition, Action trueValue, Action falseValue)
        {
            if (condition == null) throw new ArgumentNullException("condition");
            if (trueValue == null) throw new ArgumentNullException("trueValue");
            if (falseValue == null) throw new ArgumentNullException("falseValue");

            var trueLabel = _writer.DefineLabel();
            var doneLabel = _writer.DefineLabel();

            var c = condition();
            GoToIf(c, trueLabel);
            int count = _evalStack.Count;
            falseValue();
            if (_evalStack.Count != count + 1)
            {
                throw new StackValidationException("The ternary operator expects exactly one more item on the evaluation stack after falseValue completes");
            }
            var falseResult = _evalStack.Pop();
            StackAssert.IsAssignable(falseResult, new StackItem(type, options), true);

            _writer.Emit(OpCodes.Br, doneLabel);
            trueLabel.Mark();
            count = _evalStack.Count();
            trueValue();
            if (_evalStack.Count != count + 1)
            {
                throw new StackValidationException("The ternary operator expects exactly one more item on the evaluation stack after trueValue completes");
            }
            var trueResult = _evalStack.Pop();
            StackAssert.IsAssignable(trueResult, new StackItem(type, options), true);

            doneLabel.Mark();

            _evalStack.Push(type, options);
        }

        public Type ReturnType
        {
            get { return _header.ReturnType; }
        }

        public IVariable This
        {
            get
            {
                if (_thisParameter == null)
                {
                    if ((_header.Attributes & CallingConventions.HasThis) != CallingConventions.HasThis)
                    {
                        throw new InvalidOperationException("MethodAttributes.HasThis must be set");
                    }
                    _thisParameter = new Parameter(this, _writer.GetParameter(0));
                }
                return _thisParameter;
            }
        }

        public IVariable GetParameter(int index)
        {
            return _parameters(index);
        }

        public void Cast(Type toType)
        {
            if (toType.IsValueType)
            {
                var item = _evalStack.Peek();
                if (item.ItemType != ItemType.Value)
                {
                    this.UnboxAny(toType);
                }
            }
            else
            {
                var item = _evalStack.Peek();
                if (item.ItemType != ItemType.Reference)
                {
                    this.Box();
                }

                item = _evalStack.Pop();
                if (item.ItemType != ItemType.Reference)
                {
                    throw new InvalidOperationException("预期的是引用类型");
                }

                _writer.Emit(OpCodes.Castclass, toType);
                _evalStack.Push(toType);
            }
        }

      
        public MethodGenerator Store(IVariable variable)
        {
            ArgumentAssert.IsNotNull(variable, "variable");
            variable.Store();
            return this;
        }

        public void BeginScope()
        {
            PushScope(new Scope(this));
        }

        public void EndScope()
        {
            PopScope(typeof(Scope), "BeginScope/EndScope mismatch");
        }

        public IVariable BeginUsing()
        {
            var scope = new UsingScope(this);
            PushScope(scope);
            return scope.UsingTarget;
        }

        public void EndUsing()
        {
            PopScope(typeof(UsingScope), "BeginUsing/EndUsing mismatch");
        }

        public void InitValue()
        {
            var item = _evalStack.Pop();
            if (item.ItemType != ItemType.AddressToValue)
            {
                throw new StackValidationException("预期加载的方式是address to a value 类型.");
            }
            _writer.Emit(OpCodes.Initobj, item.Type);
        }

        /// <summary>
        /// 检查参数合法性，并从模拟计算堆栈中弹出参数
        /// </summary>
        /// <param name="method"></param>
        private void PopAndValidateParams(MethodBase method)
        {
            var parameters = method.GetParameters();
            for (int i = parameters.Length - 1; i >= 0; --i)
            {
                // 验证参数是否合法
                StackAssert.IsAssignable(_evalStack.Pop(), new StackItem(parameters[i].ParameterType, LoadOptions.Default), true);
            }
            if (!method.IsStatic && !method.IsConstructor)
            {
                //如果不是静态方法或者构造方法，那么当前栈顶的数据，应该是this
                //即：方法所声明的类型
                StackAssert.IsAssignable(_evalStack.Pop(), new StackItem(method.DeclaringType, LoadOptions.ValueAsAddress), false);
            }
        }

        /// <summary>
        /// <para>如果栈顶部是值对象，那么该方法可以将顶部的对象拆箱成<paramref name="type"/>对应的值类型</para>
        /// <para>如果栈顶部是引用对象，那么该方法可以顶部的对象转换成<paramref name="type"/>对应的引用类型</para>
        /// <para>转换后的值会被存储在堆栈上</para>
        /// </summary>
        public MethodGenerator UnboxAny(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            var item = _evalStack.Pop();
            StackAssert.IsAssignable(item, new StackItem(typeof(object), LoadOptions.BoxValues), false);
            _writer.Emit(OpCodes.Unbox_Any, type);
            _evalStack.Push(type);
            return this;
        }

        /// <summary>
        /// Emits instructions to load a <see cref="Type"/> reference onto the evaluation stack.
        /// </summary>
        /// <param name="type">The type instance to load.</param>
        /// <exception cref="ArgumentNullException">
        ///		<para><paramref name="type"/> is <see langword="null"/>.</para>
        /// </exception>
        public MethodGenerator Load(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            var method = typeof(Type).ResolveMethod("GetTypeFromHandle", typeof(RuntimeTypeHandle));
            if (method == null)
            {
                throw new MissingMethodException(typeof(Type).FullName, "GetTypeFromHandle");
            }
            LoadToken(type);
            Call(method);
            return this;
        }

        /// <summary>
        /// Emits instructions to load a <see cref="RuntimeTypeHandle"/> for <paramref name="type"/> onto the evaluation stack.
        /// </summary>
        /// <param name="type">The type to load.</param>
        /// <exception cref="ArgumentNullException">
        ///		<para><paramref name="type"/> is <see langword="null"/>.</para>
        /// </exception>
        public MethodGenerator LoadToken(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            _writer.Emit(OpCodes.Ldtoken, type);
            _evalStack.Push(typeof(RuntimeTypeHandle));
            return this;
        }

        /// <summary>
        /// Emits instructions to load a <see cref="RuntimeMethodHandle"/> for <paramref name="method"/> onto the evaluation stack.
        /// </summary>
        /// <param name="method">The method to load.</param>
        /// <exception cref="ArgumentNullException">
        ///		<para><paramref name="method"/> is <see langword="null"/>.</para>
        /// </exception>
        public MethodGenerator LoadToken(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException("method");
            _writer.Emit(OpCodes.Ldtoken, method);
            _evalStack.Push(typeof(RuntimeMethodHandle));
            return this;
        }

        /// <summary>
        /// Emits instructions to load a <see cref="RuntimeFieldHandle"/> for <paramref name="field"/> onto the evaluation stack.
        /// </summary>
        /// <param name="field">The method to load.</param>
        /// <exception cref="ArgumentNullException">
        ///		<para><paramref name="field"/> is <see langword="null"/>.</para>
        /// </exception>
        public MethodGenerator LoadToken(FieldInfo field)
        {
            if (field == null) throw new ArgumentNullException("field");
            _writer.Emit(OpCodes.Ldtoken, field);
            _evalStack.Push(typeof(RuntimeFieldHandle));
            return this;
        }

        /// <summary>
        /// Emits instructions to load a <see cref="RuntimeFieldHandle"/> or <see cref="RuntimeMethodHandle"/> for <paramref name="member"/> onto the evaluation stack.
        /// </summary>
        /// <param name="member">The method or field to load.</param>
        /// <exception cref="ArgumentNullException">
        ///		<para><paramref name="member"/> is <see langword="null"/>.</para>
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		<para><paramref name="member"/> is not a field or method.</para>
        /// </exception>
        public MethodGenerator LoadToken(MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException("member");
            if (member is MethodInfo)
            {
                LoadToken((MethodInfo)member);
            }
            else if (member is FieldInfo)
            {
                LoadToken((FieldInfo)member);
            }
            else
            {
                throw new ArgumentException("member must be a field or method", "member");
            }
            return this;
        }

        /// <summary>
        /// Begins a try block.
        /// </summary>
        public void Try()
        {
            PushScope(new Scope(this));
            _writer.BeginTry();
        }

        /// <summary>
        /// Begins a catch block. Must have previously called <see cref="Try"/>.
        /// </summary>
        public void Catch()
        {
            PopScope(typeof(Scope), "Catch without try");
            PushScope(new Scope(this));
            _writer.BeginCatch(typeof(object));
            _writer.Emit(OpCodes.Pop);
        }

        /// <summary>
        /// Begins a catch block that filters for a particular exception type.
        /// Must have previously called <see cref="Try"/>.
        /// Returns a variable that can be used to access the caught exception.
        /// </summary>
        public IVariable Catch(Type exceptionType)
        {
            PopScope(typeof(Scope), "Catch without try");
            PushScope(new Scope(this));
            var ex = DefineLocal(exceptionType, false, "ex");
            BeginAssign(ex);
            _writer.BeginCatch(exceptionType);
            _evalStack.Push(exceptionType);
            EndAssign();
            return ex;
        }

        /// <summary>
        /// Begins a finally block.
        /// </summary>
        public void Finally()
        {
            PopScope(typeof(Scope), "Finally without try");
            PushScope(new Scope(this));
            _writer.BeginFinally();
        }

        /// <summary>
        /// Ends any try/catch/finally block.
        /// </summary>
        public void EndTryCatchFinally()
        {
            PopScope(typeof(Scope), "End of try/catch/finally within an invalid scope");
            _writer.EndTryCatchFinally();
        }

        private static bool ShouldLoadAddress(Type typeToLoad, LoadOptions options)
        {
            if (typeToLoad.IsValueType)
            {
                return (options & LoadOptions.ValueAsAddress) == LoadOptions.ValueAsAddress;
            }
            return (options & LoadOptions.ReferenceAsAddress) == LoadOptions.ReferenceAsAddress;
        }

        /// <summary>
        /// 封装了返回指令
        /// </summary>
        public void Return()
        {
            if (_header.ReturnType != typeof(void))
                StackAssert.IsAssignable(_evalStack.Pop(), new StackItem(_header.ReturnType, LoadOptions.Default), true);

            if (_evalStack.Count > 0)
                throw new InvalidOperationException("在返回代码执行之前，计算堆栈上的所有数据应该已被移除.");

            _writer.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// 定义局部变量
        /// </summary>
        /// <param name="type">变量类型</param>
        /// <param name="isPinned">变量是否是固定的</param>
        /// <param name="name">变量名称</param>
        /// <returns>返回局部变量的包装</returns>
        private LocalDefinition DefineLocal(Type type, bool isPinned, string name)
        {
            var result = new LocalDefinition(this, _writer.DefineLocal(type, isPinned, name));
            _locals.Add(result);
            return result;
        }

        /// <summary>
        /// <para>从已定义的变量集合中，借一个局部变量</para>
        /// <para>可以借出的变量，需要满足2个条件：1.变量已失效（在声明代码块范围之外）2.变量类型相同（类型、是否固定，这两个属性要相同）</para>
        /// <para>借出机制是为了节省创建变量的内存消耗，对已创建过的变量进行缓存，重复利用</para>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isPinned"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private BorrowedLocal BorrowLocal(Type type, bool isPinned, string name)
        {
            foreach (var localDef in _locals)
            {
                if (!localDef.InScope
                    && localDef.Type == type
                    && localDef.IsPinned == isPinned)
                {
                    return new BorrowedLocal(localDef);
                }
            }
            return new BorrowedLocal(DefineLocal(type, isPinned, name));
        }





        /// <summary>
        /// 获取字段或属性的信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        private static MemberInfo GetFieldOrProperty(Type type, string memberName)
        {
            //常量
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            MemberInfo result = type.GetField(memberName, flags);
            if (result != null) return result;
            result = type.GetProperty(memberName, flags);
            if (result != null) return result;

            throw new InvalidOperationException(string.Format("字段和属性 {0}.{1} 不存在.", type.FullName, memberName));
        }

        /// <summary>
        /// 将新的代码范围压入范围堆栈
        /// </summary>
        /// <param name="newScope"></param>
        private void PushScope(Scope newScope)
        {
            _scopeStack.Push(_currentScope);
            _currentScope = newScope;
            newScope.Open();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expectedScopeType">预期的范围类型</param>
        /// <param name="message"></param>
        private void PopScope(Type expectedScopeType, string message)
        {
            if (_scopeStack.Count == 0)
            {
                throw new InvalidOperationException("弹出范围的次数过多");
            }

            if (expectedScopeType != null && _currentScope.GetType() != expectedScopeType)
            {
                //如果 ?? 运算符的左操作数非空，该运算符将返回左操作数，否则返回右操作数。
                throw new InvalidOperationException(message ?? string.Format(
                    "预计目前的范围类型为 '{0}' 但是实际是 '{1}'",
                    expectedScopeType.Name, _currentScope.GetType().Name));
            }

            _currentScope.Dispose();
            _currentScope = _scopeStack.Pop();
        }

        /// <summary>
        /// 加载参数，并写入条件转移指令
        /// </summary>
        /// <param name="op"></param>
        /// <param name="target"></param>
        private void GoToIf(LogicOperator op, ILabel target)
        {
            for (int i = 0; i < op.ArgumentCount; ++i)
            {
                // 参与验证的项
                _evalStack.Pop();
            }
            op.WriteBranch(_writer, target);
        }

        [DebuggerDisplay("VariableCount={_ownedLocals.Count}, EvalStackDepth={_evalStack.Count}")]
        private class Scope : IDisposable
        {
            private readonly MethodGenerator _owner;
            private readonly List<BorrowedLocal> _ownedLocals = new List<BorrowedLocal>();

            /// <summary>
            /// 代码段开始时，计算堆栈的深度
            /// </summary>
            private int _startEvalStackDepth;

            /// <summary>
            /// <para>该字段表示，在代码段结束后，相对于刚进入代码段的时候，多出的堆栈空间</para>
            /// <para>比如说，在该代码段里，执行了语句加载一个int数据，这时候代码段结束，那么堆栈就多出了1的空间</para>
            /// </summary>
            private int _evalStackOverflow;
            private bool _closed;

            public Scope(MethodGenerator owner)
            {
                _owner = owner;
            }

            public Scope(MethodGenerator owner, int evalStackOverflow)
                : this(owner)
            {
                _evalStackOverflow = evalStackOverflow;
            }

            protected MethodGenerator Owner
            {
                [DebuggerStepThrough]
                get { return _owner; }
            }

            public IVariable DeclareLocal(Type type, bool isPinned)
            {
                return DeclareLocal(type, isPinned, null);
            }

            public IVariable DeclareLocal(Type type, bool isPinned,string name)
            {
                var result = _owner.BorrowLocal(type, isPinned, name);
                _ownedLocals.Add(result);
                return result;
            }

            public virtual void Open()
            {
                _startEvalStackDepth = Owner._evalStack.Count;
            }

            /// <summary>
            /// 结束代码段
            /// </summary>
            protected virtual void Close()
            {
                if (_closed) return;

                try
                {
                    //释放该代码段的局部变量
                    foreach (var local in _ownedLocals)
                    {
                        local.Dispose();
                    }

                    if (Owner._evalStack.Count != (_startEvalStackDepth + _evalStackOverflow))
                    {
                        throw new InvalidOperationException(string.Format("Begin/End 代码段验证失败 - 开始代码段时，有 {0} 项在计算堆栈，但是结束时，还有 {1} 项，预期应该有 {2} 项",
                                                                                                _startEvalStackDepth, Owner._evalStack.Count, _startEvalStackDepth + _evalStackOverflow));
                    }
                }
                finally
                {
                    _closed = true;
                }
            }

            public void Dispose()
            {
                Close();
            }
        }

        /// <summary>
        /// <para>将局部变量封装到BorrowedLocal</para>
        /// <para>这样可以重用已脱离代码段的局部变量</para>
        /// </summary>
        [DebuggerDisplay("Type={Type.Name}, IsPinned={IsPinned}")]
        private sealed class BorrowedLocal : IVariable, IDisposable
        {
            private readonly LocalDefinition _owner;
            private bool _outOfScope;

            public BorrowedLocal(LocalDefinition owner)
            {
                _owner = owner;
                _owner.InScope = true;
            }

            public Type Type
            {
                [DebuggerStepThrough]
                get { return _owner.Type; }
            }

            public bool IsPinned
            {
                [DebuggerStepThrough]
                get { return _owner.IsPinned; }
            }

            public string Name
            {
                [DebuggerStepThrough]
                get { return _owner.Name; }
            }

            [DebuggerStepThrough]
            public void Initialize()
            {
                _owner.Initialize();
            }

            public void Load(LoadOptions options)
            {
                ValidateScope();
                _owner.Load(options);
            }

            public void Store()
            {
                ValidateScope();
                _owner.Store();
            }

            public bool CanStore
            {
                get
                {
                    ValidateScope();
                    return _owner.CanStore;
                }
            }

            public void BeginAssign()
            {
                ValidateScope();
                _owner.BeginAssign();
            }

            public void EndAssign()
            {
                ValidateScope();
                _owner.EndAssign();
            }

            private void ValidateScope()
            {
                if (_outOfScope) throw new InvalidOperationException("局部变量已脱离了代码范围");
            }

            public void Dispose()
            {
                //释放局部变量
                //局部变量并没有真正释放，指示把相关标记改变了
                _outOfScope = true;//指示变量脱离了代码范围
                _owner.InScope = false;//指示变量不在代码范围中
            }
        }

        private class LabeledScope : Scope
        {
            public LabeledScope(MethodGenerator owner)
                : base(owner)
            {
                EndLabel = owner._writer.DefineLabel();
            }

            public ILabel EndLabel { get; protected set; }

            protected override void Close()
            {
                EndLabel.Mark();
                base.Close();
            }
        }

        private class UsingScope : Scope
        {
            private MethodInfo _targetDisposeMethod;

            public UsingScope(MethodGenerator owner)
                : base(owner)
            {
            }

            public override void Open()
            {
                var item = Owner._evalStack.Peek();
                StackAssert.AreEqual(item, new StackItem(item.Type, LoadOptions.Default));

                _targetDisposeMethod = UsingTarget.Type.GetBestCallableOverride(_disposeMethod);

                UsingTarget = Owner.Declare(item.Type);
                Owner.Store(UsingTarget);

                base.Open();

                Owner._writer.BeginTry();
            }

            public IVariable UsingTarget { get; private set; }

            protected override void Close()
            {
                Owner._writer.BeginFinally();
                {
                    Owner.Load(UsingTarget);
                    Owner.BeginCall(_targetDisposeMethod);
                    Owner.EndCall();
                }
                Owner._writer.EndTryCatchFinally();
                base.Close();
            }
        }

        private class ForEachScope : Scope
        {
            private static readonly MethodInfo _dispose = typeof(IDisposable).GetMethod("Dispose");
            private static readonly MethodInfo _moveNext = typeof(IEnumerator).GetMethod("MoveNext");

            private ILabel _iterateLabel;
            private ILabel _loopBlockLabel;
            private IVariable _current;
            private IVariable _enumerator;
            private MethodInfo _getEnumerator;
            private MethodInfo _getCurrent;

            public ForEachScope(MethodGenerator owner, Type elementType)
                : base(owner)
            {
                ElementType = elementType;
            }

            public IVariable Current
            {
                get
                {
                    if (_current == null)
                    {
                        throw new InvalidOperationException("Can't access the current variable outside the scope of the foreach loop.");
                    }
                    return _current;
                }
            }

            public Type ElementType { get; private set; }

            private void Initialize(StackItem enumerableItem)
            {
                Type enumerableType = typeof(IEnumerable<>).MakeGenericType(ElementType);
                Type enumeratorType;

                if (enumerableType.IsAssignableFrom(enumerableItem.Type))
                {
                    enumeratorType = typeof(IEnumerator<>).MakeGenericType(ElementType);
                }
                else
                {
                    enumerableType = typeof(IEnumerable);
                    enumeratorType = typeof(IEnumerator);
                }

                if (enumerableItem.Type.IsArray)
                    _getEnumerator = enumerableType.GetMethod("GetEnumerator");//.....
                else
                    _getEnumerator = enumerableItem.Type.GetBestCallableOverride(enumerableType.GetMethod("GetEnumerator"));

                
                _getEnumerator = enumerableType.GetMethod("GetEnumerator");
                _getCurrent = enumeratorType.GetProperty("Current").GetGetMethod();
                _current = DeclareLocal(ElementType, false);
                _enumerator = DeclareLocal(enumeratorType, false);
            }

            public override void Open()
            {
                var item = Owner._evalStack.Peek();
                StackAssert.IsAssignable(item, new StackItem(typeof(IEnumerable), LoadOptions.ValueAsAddress), false);

                Initialize(item);

                Owner.BeginAssign(_enumerator);
                Owner.Call(_getEnumerator);
                Owner.EndAssign();

                base.Open();

                Owner._writer.BeginTry();

                _iterateLabel = Owner._writer.DefineLabel();
                Owner._writer.Emit(OpCodes.Br, _iterateLabel);
                _loopBlockLabel = Owner._writer.DefineLabel();
                _loopBlockLabel.Mark();

                Owner.BeginAssign(_current);
                {
                    Owner.Load(_enumerator);
                    Owner.BeginCall(_getCurrent);
                    Owner.EndCall();

                    if (ElementType.IsValueType && _getCurrent.ReturnType == typeof(object))
                    {
                        Owner.UnboxAny(ElementType);
                    }
                }
                Owner.EndAssign();
            }

            protected override void Close()
            {
                _iterateLabel.Mark();

                Owner.Load(_enumerator);
                Owner.BeginCall(_moveNext);
                Owner.EndCall();
                Owner.GoToIf(LogicOperator.IsTrue, _loopBlockLabel);

                Owner._writer.BeginFinally();
                Owner.Load(_enumerator);
                Owner.BeginCall(_dispose);
                Owner.EndCall();
                Owner._writer.EndTryCatchFinally();

                base.Close();
                _current = null;
            }
        }

        /// <summary>
        /// 局部变量的包装
        /// </summary>
        [DebuggerDisplay("InScope={InScope}, Type={Type.Name}, IsPinned={IsPinned}")]
        private class LocalDefinition : IVariable
        {
            private readonly MethodGenerator _owner;
            private readonly IVariable _innerVariable;

            public LocalDefinition(MethodGenerator owner, IVariable innerVariable)
            {
                _owner = owner;
                _innerVariable = innerVariable;
                InScope = true;
            }

            public bool InScope { get; set; }

            public Type Type
            {
                [DebuggerStepThrough]
                get { return _innerVariable.Type; }
            }

            public bool IsPinned
            {
                [DebuggerStepThrough]
                get { return _innerVariable.IsPinned; }
            }

            public string Name
            {
                [DebuggerStepThrough]
                get { return _innerVariable.Name; }
            }

            public void Initialize()
            {
                _innerVariable.Initialize();
            }

            public void Load(LoadOptions options)
            {
                _innerVariable.Load(options);
                _owner._evalStack.Push(Type, options); //维护MethodGenerator自身的计算堆栈
            }

            public void Store()
            {
                StackAssert.IsAssignable(
                    _owner._evalStack.Pop(),
                    new StackItem(Type, LoadOptions.Default),
                    true);
                _innerVariable.Store();
            }

            public bool CanStore
            {
                get { return _innerVariable.CanStore; }
            }

            public void BeginAssign()
            {
                _innerVariable.BeginAssign();
            }

            public void EndAssign()
            {
                StackAssert.IsAssignable(
                    _owner._evalStack.Pop(),
                    new StackItem(Type, LoadOptions.Default),
                    true);
                _innerVariable.EndAssign();
            }
        }

        /// <summary>
        /// 封装了在执行方法时的参数
        /// 该参数可以将值加载到堆栈中
        /// 也可以更新值到参数里
        /// ......
        /// </summary>
        [DebuggerDisplay("Type={Type.Name}, IsPinned={IsPinned}")]
        private sealed class Parameter : IVariable
        {
            private readonly MethodGenerator _owner;
            private readonly IVariable _innerParameter;

            public Parameter(MethodGenerator owner, IVariable innerParameter)
            {
                _owner = owner;
                _innerParameter = innerParameter;
            }

            /// <summary>
            /// 参数类型
            /// </summary>
            public Type Type
            {
                //使用DebuggerStepThrough可以在单步调试时，直接通过该步骤，而不需要逐步逐步的进入，节省调试时间
                [DebuggerStepThrough]
                get { return _innerParameter.Type; }
            }

            public bool IsPinned
            {
                [DebuggerStepThrough]
                get { return _innerParameter.IsPinned; }
            }

            /// <summary>
            /// 参数名称
            /// </summary>
            public string Name
            {
                [DebuggerStepThrough]
                get { return _innerParameter.Name; }
            }

            /// <summary>
            /// 将对象的所有字段初始化为空引用或基元类型的 0。
            /// </summary>
            [DebuggerStepThrough]
            public void Initialize()
            {
                _innerParameter.Initialize();
            }

            /// <summary>
            /// 加载参数的值到堆栈中
            /// </summary>
            /// <param name="options"></param>
            public void Load(LoadOptions options)
            {
                _innerParameter.Load(options);
                _owner._evalStack.Push(Type, options);
            }

            public void Store()
            {
                StackAssert.IsAssignable( //判断pop出的类型，是否可以存入该参数
                    _owner._evalStack.Pop(),
                    new StackItem(Type, LoadOptions.Default),
                    true);
                _innerParameter.Store();//如果类型安全，那么就存储
            }

            public bool CanStore
            {
                get { return _innerParameter.CanStore; }
            }

            public void BeginAssign()
            {
                _innerParameter.BeginAssign();
            }

            public void EndAssign()
            {
                StackAssert.IsAssignable(
                    _owner._evalStack.Pop(),
                    new StackItem(Type, LoadOptions.Default),
                    true);
                _innerParameter.EndAssign();
            }
        }

        [DebuggerDisplay("Expression={Name}, Type={Type.Name}")]
        private class Expression : IExpression
        {
            private readonly MethodGenerator _owner;
            private readonly IVariable _variable;
            private readonly List<MemberInfo> _members;
            private readonly Stack<Action> _delayedInstructions = new Stack<Action>();

            public Expression(MethodGenerator owner, IVariable variable)
            {
                _owner = owner;
                _variable = variable;
                _members = new List<MemberInfo>();
            }

            public void Load(LoadOptions loadOptions)
            {
                if (_members.Count == 0)
                {
                    _variable.Load(loadOptions);
                }
                else
                {
                    _variable.Load(LoadOptions.ValueAsAddress);
                    for (int i = 0; i < _members.Count - 1; ++i)
                    {
                        LoadMember(_members[i], LoadOptions.ValueAsAddress);
                    }
                    if (_members.Count > 0)
                    {
                        LoadMember(_members[_members.Count - 1], loadOptions);
                    }
                }
            }

            public void BeginAssign()
            {
                if (IsReadOnly)
                {
                    throw new InvalidOperationException("This value is read-only.");
                }

                if (_members.Count == 0)
                {
                    _variable.BeginAssign();
                    _delayedInstructions.Push(_variable.EndAssign);
                }
                else
                {
                    _variable.Load(LoadOptions.ValueAsAddress);

                    for (int i = 0; i < _members.Count - 1; ++i)
                    {
                        LoadMember(_members[i], LoadOptions.ValueAsAddress);
                    }
                    //var count = _owner._delayedInstructions.Count;
                    _owner.BeginAssign(_members[_members.Count - 1]);
                    //Debug.Assert(count < _owner._delayedInstructions.Count, "MethodGenerator.BeginAssign did not push any instructions onto _delayInstructions as expected");
                    //var endInstruction = _owner._delayedInstructions.Pop();
                    _delayedInstructions.Push(_owner.EndAssign);
                }
            }

            public void EndAssign()
            {
                _delayedInstructions.Pop()();
            }

            public IExpression Copy()
            {
                var result = new Expression(_owner, _variable);
                result._members.AddRange(_members);
                return result;
            }

            public IExpression AddMember(string memberName)
            {
                if (memberName == null) throw new ArgumentNullException("memberName");
                ValidateIsModifiable();

                var field = Type.GetField(memberName);
                if (field != null)
                {
                    return AddMember(field);
                }

                var property = Type.GetProperty(memberName);
                if (property != null)
                {
                    return AddMember(property);
                }

                throw new MissingMemberException(string.Format("Type '{0}' does not define a field or property named '{1}'", Type.Name, memberName));
            }

            public Type Type
            {
                get
                {
                    if (_members.Count == 0) return _variable.Type;

                    var lastMember = _members[_members.Count - 1];
                    if (lastMember is FieldInfo)
                    {
                        return ((FieldInfo)lastMember).FieldType;
                    }
                    return ((PropertyInfo)lastMember).PropertyType;
                }
            }

            public IExpression AddMember(FieldInfo field)
            {
                if (field == null) throw new ArgumentNullException("field");
                ValidateIsModifiable();

                _members.Add(field);
                return this;
            }

            public IExpression AddMember(PropertyInfo property)
            {
                if (property == null) throw new ArgumentNullException("property");
                ValidateIsModifiable();

                _members.Add(property);
                return this;
            }

            private void ValidateIsModifiable()
            {
                if (_delayedInstructions.Count > 0)
                {
                    throw new InvalidOperationException("This shortcut cannot be modified between BeginAssign/EndAssign calls.");
                }
            }

            private void LoadMember(MemberInfo member, LoadOptions options)
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    _owner.LoadField(field, options);
                }
                else
                {
                    var property = (PropertyInfo)member;
                    _owner.LoadProperty(property, options);
                }
            }

            public bool IsReadOnly { get; private set; }

            public IExpression MakeReadOnly()
            {
                IsReadOnly = true;
                return this;
            }

            public bool IsPinned
            {
                get { return _members.Count == 0 && _variable.IsPinned; }
            }

            public string Name
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(
                        string.IsNullOrEmpty(_variable.Name)
                        ? "(UnknownVariable)"
                        : _variable.Name);

                    foreach (var member in _members)
                    {
                        builder.Append(".");
                        builder.Append(member.Name);
                    }

                    return builder.ToString();
                }
            }

            public void Initialize()
            {
                if (_members.Count == 0)
                {
                    _variable.Initialize();
                }
                else
                {
                    BeginAssign();
                    _owner.LoadDefaultOf(Type);
                    EndAssign();
                }
            }

            public void Store()
            {
                if (!CanStore)
                {
                    throw new InvalidOperationException("Can't store when Type is a ByRef type.");
                }
                _variable.Store();
            }

            public bool CanStore
            {
                get { return _members.Count == 0 && _variable.CanStore; }
            }
        }
    }
}
