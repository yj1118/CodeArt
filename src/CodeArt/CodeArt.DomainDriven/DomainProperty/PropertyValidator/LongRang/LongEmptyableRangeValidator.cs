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
    public class LongEmptyableRangeValidator : PropertyValidator<Emptyable<long>>
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

        internal LongEmptyableRangeValidator(long min, long max)
        {
            this.Min = min;
            this.Max = max;
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, Emptyable<long> propertyValue, ValidationResult result)
        {
            if (propertyValue.IsEmpty()) return;
            var value = propertyValue.Value;
            if (value < this.Min)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueLessThan, property.Call, this.Min));
            else if (value > this.Max)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueMoreThan, property.Call, this.Max));
        }

        public const string ErrorCode = "LongRangeError";
        

    }
}
