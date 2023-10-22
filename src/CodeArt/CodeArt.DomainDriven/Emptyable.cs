using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.DTO;

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
    public struct Emptyable<T> : IEmptyable,IDTOSerializable
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
            if (this.IsEmpty()) return null;
            return this.Value;
        }

        public bool IsEmpty()
        {
            return !_value.HasValue;
        }

        public bool IsNull()
        {
            return this.IsEmpty();
        }

        public Emptyable(T value)
        {
            _value = value;
        }

        public Emptyable(T? value)
        {
            _value = value;
        }

        /// <summary>
        /// 将自身的内容序列化到<paramref name="owner"/>中，
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name">目标对象作为成员的名称</param>
        public void Serialize(DTObject owner, string name)
        {
            if (this.IsEmpty()) return;
            owner.SetValue(name, _value.Value);
        }

        public DTObject GetData()
        {
            if (this.IsEmpty()) return DTObject.Empty;
            DTObject dto = DTObject.Create();
            dto.SetValue(_value.Value);
            return dto;
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
            if (value.IsEmpty()) throw new InvalidOperationException(Strings.NotIncludeValue);
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
