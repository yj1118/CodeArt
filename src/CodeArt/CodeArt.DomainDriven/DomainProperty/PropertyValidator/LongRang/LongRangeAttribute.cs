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
    public class LongRangeAttribute : PropertyValidatorAttribute
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

        public LongRangeAttribute()
        {
        }

        public LongRangeAttribute(long min, long max)
        {
            this.Min = min;
            this.Max = max;
        }
        public override IPropertyValidator CreateValidator()
        {
            return new LongRangeValidator(this.Min, this.Max);
        }

    }
}