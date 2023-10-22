using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 范围验证
    /// </summary>
    public class FloatEmptyableRangeAttribute : PropertyValidatorAttribute
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

        public FloatEmptyableRangeAttribute()
        {
        }

        public FloatEmptyableRangeAttribute(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }
        public override IPropertyValidator CreateValidator()
        {
            return new FloatEmptyableRangeValidator(this.Min, this.Max);
        }

    }
}