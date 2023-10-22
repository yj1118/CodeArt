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
    public class LongRangeValidator : PropertyValidator<long>
    {
        /// <summary>
        /// 最小长度
        /// </summary>
        public long Min
        {
            get;
            private set;
        }

        /// <summary>
        ///  最大长度
        /// </summary>
        public long Max
        {
            get;
            private set;
        }

        internal LongRangeValidator(long min, long max)
        {
            this.Min = min;
            this.Max = max;
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, long propertyValue, ValidationResult result)
        {
            if (propertyValue < this.Min)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueLessThan, property.Call, this.Min));
            else if (propertyValue > this.Max)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueMoreThan, property.Call, this.Max));
        }

        public const string ErrorCode = "LongRangeError";
        

    }
}
