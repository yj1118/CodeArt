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
    public class IntEmptyableRangeAttribute : PropertyValidatorAttribute
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

        public IntEmptyableRangeAttribute()
            : this(int.MinValue,int.MaxValue)
        {
            
        }

        public IntEmptyableRangeAttribute(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }
        public override IPropertyValidator CreateValidator()
        {
            return new IntEmptyableRangeValidator(this.Min, this.Max);
        }

    }
}