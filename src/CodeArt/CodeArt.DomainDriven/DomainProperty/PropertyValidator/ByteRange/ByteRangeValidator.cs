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
    public class ByteRangeValidator : PropertyValidator<byte>
    {
        /// <summary>
        /// 最小值
        /// </summary>
        public byte Min
        {
            get;
            private set;
        }

        /// <summary>
        ///  最大值
        /// </summary>
        public byte Max
        {
            get;
            private set;
        }

        internal ByteRangeValidator(byte min, byte max)
        {
            this.Min = min;
            this.Max = max;
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, byte propertyValue, ValidationResult result)
        {
            if (propertyValue < this.Min)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueLessThan, property.Call, this.Min));
            else if (propertyValue > this.Max)
                result.AddError(property.Name, ErrorCode, string.Format(Strings.ValueMoreThan, property.Call, this.Max));
        }

        public const string ErrorCode = "ByteRangeError";
        

    }
}
