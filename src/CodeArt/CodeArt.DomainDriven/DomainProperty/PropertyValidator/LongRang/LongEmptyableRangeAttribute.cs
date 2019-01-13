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
    public class LongEmptyableRangeAttribute : PropertyValidatorAttribute
    {
        /// <summary>
        /// 最小数值
        /// </summary>
        public long Min
        {
            get;
            set;
        }

        /// <summary>
        ///  最大数值
        /// </summary>
        public long Max
        {
            get;
            set;
        }

        public LongEmptyableRangeAttribute()
        {
        }

        public LongEmptyableRangeAttribute(long min, long max)
        {
            this.Min = min;
            this.Max = max;
        }
        public override IPropertyValidator CreateValidator()
        {
            return new LongEmptyableRangeValidator(this.Min, this.Max);
        }

    }
}