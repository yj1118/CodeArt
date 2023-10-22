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
    public class FloatRangeValidator : PropertyValidator<float>
    {
        /// <summary>
        /// 最小值
        /// </summary>
        public float Min
        {
            get;
            private set;
        }

        /// <summary>
        ///  最大值
        /// </summary>
        public float Max
        {
            get;
            private set;
        }

        internal FloatRangeValidator(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, float propertyValue, ValidationResult result)
        {
            if (propertyValue < this.Min)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueLessThan, property.Call, this.Min));
            else if (propertyValue > this.Max)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueMoreThan, property.Call, this.Max));
        }

        public const string ErrorCode = "FloatRangeError";
        

    }
}
