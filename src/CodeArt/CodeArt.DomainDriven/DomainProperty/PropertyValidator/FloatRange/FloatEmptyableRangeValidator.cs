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
    public class FloatEmptyableRangeValidator : PropertyValidator<Emptyable<float>>
    {
        /// <summary>
        /// 最小长度
        /// </summary>
        public float Min
        {
            get;
            private set;
        }

        /// <summary>
        ///  最大长度
        /// </summary>
        public float Max
        {
            get;
            private set;
        }

        internal FloatEmptyableRangeValidator(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, Emptyable<float> propertyValue, ValidationResult result)
        {
            if (propertyValue.IsEmpty()) return;
            var value = propertyValue.Value;
            if (value < this.Min)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueLessThan, property.Call, this.Min));
            else if (value > this.Max)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueMoreThan, property.Call, this.Max));
        }

        public const string ErrorCode = "FloatRangeError";
        

    }
}
