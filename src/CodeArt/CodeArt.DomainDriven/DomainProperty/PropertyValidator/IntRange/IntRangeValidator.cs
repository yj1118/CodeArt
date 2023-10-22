using System;
using System.Collections.Generic;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 
    /// </summary>
    public class IntRangeValidator : PropertyValidator<int>
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

        internal IntRangeValidator(int min, int max)
        {
            this.Min = min;
            this.Max = max;
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, int propertyValue, ValidationResult result)
        {
            if (propertyValue < this.Min)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueLessThan, property.Call, this.Min));
            else if (propertyValue > this.Max)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueMoreThan, property.Call, this.Max));
        }

        public const string ErrorCode = "IntRangeError";
        

    }
}
