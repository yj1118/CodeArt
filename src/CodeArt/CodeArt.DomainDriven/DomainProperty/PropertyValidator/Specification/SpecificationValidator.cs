using System;
using System.Reflection;

using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    public class SpecificationValidator : PropertyValidator<DomainObject>
    {
        private SpecificationValidator() { }

        protected override void Validate(DomainObject domainObject, DomainProperty property, DomainObject propertyValue, ValidationResult result)
        {
            if (propertyValue != null)
            {
                ValidationResult t = propertyValue.Validate();
                if (!t.IsSatisfied) result.AddError(ErrorCode, string.Format(Strings.DoNotMeetSpecifications, property.Call));
            }
        }

        public const string ErrorCode = "SpecificationError";


        public static readonly SpecificationValidator Instance = new SpecificationValidator();
    }
}
