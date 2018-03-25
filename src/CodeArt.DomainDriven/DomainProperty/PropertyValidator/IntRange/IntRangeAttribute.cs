using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 整数范围验证
    /// </summary>
    public class IntRangeAttribute : PropertyValidatorAttribute
    {
        /// <summary>
        /// 最小数值
        /// </summary>
        public int Min
        {
            get;
            set;
        }

        /// <summary>
        ///  最大数值
        /// </summary>
        public int Max
        {
            get;
            set;
        }

        public IntRangeAttribute()
            : this(int.MinValue,int.MaxValue)
        {
            
        }

        public IntRangeAttribute(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }
        public override IPropertyValidator CreateValidator()
        {
            return new IntRangeValidator(this.Min, this.Max);
        }

    }
}