using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 字节范围验证
    /// </summary>
    public class ByteRangeAttribute : PropertyValidatorAttribute
    {
        /// <summary>
        /// 最小数值
        /// </summary>
        public byte Min
        {
            get;
            set;
        }

        /// <summary>
        ///  最大数值
        /// </summary>
        public byte Max
        {
            get;
            set;
        }

        public ByteRangeAttribute()
            : this(byte.MinValue, byte.MaxValue)
        {
            
        }

        public ByteRangeAttribute(byte min, byte max)
        {
            this.Min = min;
            this.Max = max;
        }

        public override IPropertyValidator CreateValidator()
        {
            return new ByteRangeValidator(this.Min, this.Max);
        }

    }
}