using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

using CodeArt.Runtime;

using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    [MergeDomain]
    [FrameworkDomain]
    public abstract class ValueObject : DomainObject, IValueObject
    {
        public ValueObject()
        {
            this.OnConstructed();
        }

        /// <summary>
        /// 由于ORM存储中间表时需要用到编号，所以我们提供了该属性
        /// 该属性仅在ORM中使用，不要在领域层中出现
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get;
            private set;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TrySetId(Guid id)
        {
            if (this.Id == default(Guid))
                this.Id = id; //如果没有编号，那么值对象需要追加编号，有编号则意味着值对象在数据库中已存在
        }


        protected override void ReadOnlyCheckUp()
        {
            if (this.IsConstructing) return; //构造阶段不处理
            throw new DomainDrivenException(string.Format(Strings.ValueObjectReadOnly, this.ObjectType.FullName));
        }

        /// <summary>
        /// 要通过数据判断值对象相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var target = obj as ValueObject;
            if (target == null) return false;

            var targetType = target.ObjectType;
            if (targetType != ObjectType) return false;

            //对比所有领域属性
            var properties = DomainProperty.GetProperties(ObjectType);
            foreach (var property in properties)
            {
                if (!EqualsHelper.ObjectEquals(this.GetValue(property), target.GetValue(property)))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return HashCoder.GetCode(ObjectType, (hashCodes) =>
            {
                var properties = DomainProperty.GetProperties(ObjectType);
                foreach (var property in properties)
                {
                    var propertyValue = this.GetValue(property);

                    hashCodes.Add(HashCoder<string>.GetCode(property.Name));
                    hashCodes.Add(HashCoder.GetCode(propertyValue));
                }
            });
        }

        public static bool operator ==(ValueObject value0, ValueObject value1)
        {
            if ((object)value0 == null && (object)value1 == null) return true;
            if ((object)value0 == null || (object)value1 == null) return false;
            return value0.Equals(value1);
        }

        public static bool operator !=(ValueObject value0, ValueObject value1)
        {
            return !(value0 == value1);
        }

    }

    [MergeDomain]
    [FrameworkDomain]
    public abstract class ValueObject<TObject, TObjectEmpty> : ValueObject
        where TObject : ValueObject<TObject, TObjectEmpty>
        where TObjectEmpty : TObject, new()
    {
        public ValueObject()
        {
            this.OnConstructed();
        }

        private static TObject _empty;
        private static object _syncObject = new object();

        public static TObject Empty
        {
            get
            {
                if (_empty == null)
                {
                    lock (_syncObject)
                    {
                        if (_empty == null)
                        {
                            _empty = new TObjectEmpty();
                        }
                    }
                }
                return _empty;
            }
        }
    }

}