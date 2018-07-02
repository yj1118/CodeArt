using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 浮点数范围验证
    /// </summary>
    public class FloatRangeAttribute : PropertyValidatorAttribute
    {
        /// <summary>
        /// 最小数值
        /// </summary>
        public float Min
        {
            get;
            set;
        }

        /// <summary>
        ///  最大数值
        /// </summary>
        public float Max
        {
            get;
            set;
        }

        public FloatRangeAttribute()
            : this(float.MinValue, float.MaxValue)
        {
            
        }

        public FloatRangeAttribute(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }
        public override IPropertyValidator CreateValidator()
        {
            return new FloatRangeValidator(this.Min, this.Max);
        }

    }
}