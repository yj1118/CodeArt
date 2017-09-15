using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public interface IEmptyable : INotNullObject
    {
        object GetValue();
    }

    /// <summary>
    /// 我们是无法判断值类型的数据是否为 empty的，因为值类型的每个值都有可能有意义，
    /// 因此我们提供了Emptyable结构，来帮助我们完成值类型数据为Empty的判断
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Emptyable<T> : IEmptyable
        where T : struct
    {
        private T? _value;

        public T Value
        {
            get
            {
                return _value.Value;
            }
        }

        public object GetValue()
        {
            return this.Value;
        }

        public bool IsEmpty()
        {
            return !_value.HasValue;
        }

        public Emptyable(T value)
        {
            _value = value;
        }

        public Emptyable(T? value)
        {
            _value = value;
        }

        public override string ToString()
        {
            if (!_value.HasValue) return string.Empty;
            return this.Value.ToString();
        }


        public static implicit operator Emptyable<T>(T value)
        {
            return new Emptyable<T>(value);
        }


        public static implicit operator T(Emptyable<T> value)
        {
            return value.Value;
        }


        public static implicit operator Emptyable<T>(T? value)
        {
            return new Emptyable<T>(value);
        }


        public static implicit operator T? (Emptyable<T> value)
        {
            return value.Value;
        }

        public static Emptyable<T> CreateEmpty()
        {
            return default(Emptyable<T>);
        }

        public readonly static Type ValueType = typeof(T);
    }
}
