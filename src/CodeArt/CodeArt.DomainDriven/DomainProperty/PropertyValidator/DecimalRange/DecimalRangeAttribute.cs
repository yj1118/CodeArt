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
    public class DecimalRangeAttribute : PropertyValidatorAttribute
    {
        /// <summary>
        /// 最小数值
        /// </summary>
        public decimal Min
        {
            get;
            set;
        }

        /// <summary>
        ///  最大数值
        /// </summary>
        public decimal Max
        {
            get;
            set;
        }

        public DecimalRangeAttribute()
            : this((double)decimal.MinValue, (double)decimal.MaxValue)
        {
            
        }

        public DecimalRangeAttribute(double min, double max)
        {
            this.Min = (decimal)min;
            this.Max = (decimal)max;
        }
        public override IPropertyValidator CreateValidator()
        {
            return new DecimalRangeValidator(this.Min, this.Max);
        }

    }
}