using System;
using System.Diagnostics;

namespace CodeArt.Runtime.IL
{
    internal struct StackItem : IEquatable<StackItem>
    {
        public static bool operator ==(StackItem a, StackItem b)
        {
            if ((object)a == null) return (object)b == null;
            if ((object)b == null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(StackItem a, StackItem b)
        {
            return !(a == b);
        }

        private readonly Type _type;
        /// <summary>
        /// 是否可以装箱
        /// </summary>
        private readonly bool _boxed;
        private Type _normalType;

        public StackItem(Type type)
            : this(type, LoadOptions.Default)
        {
        }

        public StackItem(Type type, LoadOptions options)
        {
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);

            _boxed = options.ShouldBox(type);

            type = type.ResolveByRef(options);

            _type = type;
            //下句的意思是，如果type是一个由引用传递的类型，那么就获取它实际引用的对象的类型
            //否则，就使用type本身
            _normalType = _type.IsByRef ? _type.GetElementType() : _type;
        }

        /// <summary>
        /// <para>获取一个值，通过该值指示 System.Type 是否由引用传递。</para>
        /// </summary>
        public bool IsAddress
        {
            get { return _type.IsByRef; }
        }

        /// <summary>
        /// 堆栈存储的项的类型
        /// </summary>
        public ItemType ItemType
        {
            get
            {
                return _type.IsByRef
                    ? LoadOptions.AnyAsAddress.GetItemType(Type, _boxed)
                    : LoadOptions.Default.GetItemType(Type, _boxed);
            }
        }

        /// <summary>
        /// <para>堆栈存储的项的常规类型</para>
        /// <para>常规类型是指：引用类型或值类型，而不是指针地址类型</para>
        /// <para>如果是指针，那么常规类型就是该指针指向的对象的实际类型</para>
        /// </summary>
        public Type Type
        {
            [DebuggerStepThrough]
            get { return _normalType; }
        }

        #region IEquatable<EvalStackItem> Members

        public bool Equals(StackItem other)
        {
            return _boxed == other._boxed && _type == other._type;
        }

        public override bool Equals(object obj)
        {
            return Equals((StackItem)obj);
        }

        public override int GetHashCode()
        {
            return _boxed.GetHashCode() ^ _type.GetHashCode();
        }

        #endregion

        public override string ToString()
        {
            return string.Format("ItemType={0}, Type={1}", ItemType, Type == null ? "null" : Type.Name);
        }
    }
}
