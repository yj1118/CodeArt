using System;
using System.Collections.Generic;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 如果指定了min大于0，则自动验证字符串不得为空
    /// </summary>
    public class StringLengthValidator : PropertyListabelValidator<string>
    {
        /// <summary>
        /// 最小长度
        /// </summary>
        public int Min
        {
            get;
            private set;
        }

        /// <summary>
        ///  最大长度
        /// </summary>
        public int Max
        {
            get;
            private set;
        }

        public StringLengthValidator(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, string propertyValue, ValidationResult result)
        {
            if (propertyValue != null)
            {
                int length = propertyValue.Length;
                if (length < this.Min)
                    result.AddError(property.Name, ErrorCode, string.Format(Strings.StringLengthLessThan, property.Call, this.Min));
                else if (length > this.Max)
                    result.AddError(property.Name, ErrorCode, string.Format(Strings.StringLengthMoreThan, property.Call, this.Max));
            }
        }

        public const string ErrorCode = "StringLengthError";
        

    }
}
