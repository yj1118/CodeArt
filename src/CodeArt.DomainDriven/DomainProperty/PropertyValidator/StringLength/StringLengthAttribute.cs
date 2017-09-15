using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 如果指定了min大于0，则自动验证字符串不得为空
    /// </summary>
    public class StringLengthAttribute : PropertyValidatorAttribute
    {
        /// <summary>
        /// 最小长度
        /// </summary>
        public int Min
        {
            get;
            set;
        }

        /// <summary>
        ///  最大长度
        /// </summary>
        public int Max
        {
            get;
            set;
        }

        public StringLengthAttribute()
        {
        }

        public StringLengthAttribute(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }
        public override IPropertyValidator CreateValidator()
        {
            return new StringLengthValidator(this.Min, this.Max);
        }

    }
}